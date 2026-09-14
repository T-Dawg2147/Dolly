using Dapper;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dolly.Infrastructure.Data;

public sealed class SearchService(IConfiguration config) : ISearchService
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
        ?? throw new InvalidOperationException("Missing connection string: CatalogueDb");

    public async Task<IReadOnlyList<SearchResultRow>> SearchAsync(string searchTerm, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return [];

        var normalized = searchTerm.Contains(" AND ", StringComparison.OrdinalIgnoreCase) ||
                         searchTerm.Contains(" OR ", StringComparison.OrdinalIgnoreCase)
            ? searchTerm
            : $"\"{searchTerm}\"";

        const string sql = """
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT
                *
            FROM Catalogue.fn_GetSearchResults(@Term)
            ORDER BY [Location],[FoundIn];
            """;

        await using var conn = new SqlConnection(_cs);
        var rows = await conn.QueryAsync<SearchResultRow>(
            new CommandDefinition(sql, new { Term = normalized }, cancellationToken: ct));

        return rows.AsList();
    }

    public async Task<SearchNavigationTarget?> ResolveNavigationTargetAsync(SearchResultRow row, bool reportingMode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(row.KeyValue) || string.IsNullOrWhiteSpace(row.FoundIn))
            return null;

        await using var conn = new SqlConnection(_cs);

        // Contacts
        if (row.FoundIn.StartsWith("[Catalogue].[Contacts]", StringComparison.OrdinalIgnoreCase))
        {
            var supplier = await conn.ExecuteScalarAsync<string?>(
                new CommandDefinition(
                    "SELECT TOP 1 Supplier_Code FROM Catalogue.Contacts WHERE Id = @Id",
                    new { Id = row.KeyValue }, cancellationToken: ct));

            if (string.IsNullOrWhiteSpace(supplier)) return null;
            return new SearchNavigationTarget { SupplierCode = supplier };
        }

        // Products
        if (row.FoundIn.StartsWith("[Catalogue].[Products]", StringComparison.OrdinalIgnoreCase))
        {
            var name = await conn.ExecuteScalarAsync<string?>(
                new CommandDefinition(
                    "SELECT TOP 1 Name FROM Catalogue.WorkingTable WHERE Id = @Id",
                    new { Id = row.KeyValue }, cancellationToken: ct));

            var supplierField = reportingMode ? "Reporting_Supplier" : "Supplier_Code";
            var supplier = await conn.ExecuteScalarAsync<string?>(
                new CommandDefinition(
                    $"SELECT TOP 1 {supplierField} FROM Catalogue.WorkingTable WHERE Id = @Id",
                    new { Id = row.KeyValue }, cancellationToken: ct));

            if (string.IsNullOrWhiteSpace(supplier)) return null;
            return new SearchNavigationTarget
            {
                SupplierCode = supplier,
                ProductCodeOrName = name ?? row.KeyValue
            };
        }

        // Suppliers
        if (row.FoundIn.StartsWith("[Catalogue].[Suppliers]", StringComparison.OrdinalIgnoreCase))
        {
            return new SearchNavigationTarget { SupplierCode = row.KeyValue };
        }

        // Product groups
        if (row.FoundIn.StartsWith("[Catalogue].[ProductGroups]", StringComparison.OrdinalIgnoreCase))
        {
            var productName = await conn.ExecuteScalarAsync<string?>(
                new CommandDefinition(
                    "SELECT TOP 1 Name FROM Catalogue.WorkingTable WHERE Parent_ID = @ParentId",
                    new { ParentId = row.KeyValue }, cancellationToken: ct));

            if (string.IsNullOrWhiteSpace(productName)) return null;

            var supplierField = reportingMode ? "Reporting_Supplier" : "Supplier_Code";
            var supplier = await conn.ExecuteScalarAsync<string?>(
                new CommandDefinition(
                    $"SELECT TOP 1 {supplierField} FROM Catalogue.WorkingTable WHERE Name = @Name",
                    new { Name = productName }, cancellationToken: ct));

            if (string.IsNullOrWhiteSpace(supplier)) return null;

            return new SearchNavigationTarget
            {
                SupplierCode = supplier,
                ProductCodeOrName = productName,
                GroupFilterParentId = row.KeyValue
            };
        }

        return null;
    }
}