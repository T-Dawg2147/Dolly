using Dapper;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dolly.Infrastructure.Data;

public sealed class ReportCatalogService(IConfiguration config) : IReportCatalogService
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
                                  ?? throw new InvalidOperationException("Missing connection string: CatalogueDb");

    public async Task<IReadOnlyList<ReportDefinition>> GetReportsAsync(CancellationToken ct = default)
    {
        const string sql = """
                           SELECT ReportId, Category, Title, Description, SourceObject, SourceKind,
                                  RequiresSupplierCode, RequiresReportingSupplierFlag,
                                  OutputFileName, SheetName, TitleTemplate, IconKind, SortOrder
                           FROM Catalogue.ReportCatalog
                           WHERE IsActive = 1
                           ORDER BY Category, SortOrder, Title;
                           """;

        await using var conn = new SqlConnection(_cs);
        var rows = await conn.QueryAsync<ReportDefinition>(new CommandDefinition(sql, cancellationToken: ct));
        return rows.AsList();
    }
}