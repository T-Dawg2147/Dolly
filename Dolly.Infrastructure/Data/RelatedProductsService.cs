using Dapper;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dolly.Infrastructure.Data;

/// <summary>
/// VBA parity:
/// - frm_Related / frm_RelatedProducts / frm_AddRelatedProducts
/// - usp_AddRelatedProduct (output @State)
/// - fn_RelatedProductData
/// - usp_UnlinkRelatedByRef
/// - usp_UpdateRelated
/// </summary>
public sealed class RelatedProductsService(IConfiguration config) : IRelatedProductsService
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
        ?? throw new InvalidOperationException("Missing connection string: CatalogueDb");

    public async Task<IReadOnlyList<RelatedProductRow>> GetRelatedAsync(string productCode, CancellationToken ct = default)
    {
        const string sql = """
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT *
            FROM Catalogue.RelatedProducts
            WHERE [Name] = @Code;
            """;

        await using var conn = new SqlConnection(_cs);
        var rows = await conn.QueryAsync<RelatedProductRow>(new CommandDefinition(sql, new { Code = productCode }, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<RelatedProductPreviewRow?> GetRelatedPreviewAsync(string relatedCode, CancellationToken ct = default)
    {
        const string sql = """
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT TOP 1 * FROM Catalogue.fn_RelatedProductData(@Code);
            """;

        await using var conn = new SqlConnection(_cs);
        return await conn.QueryFirstOrDefaultAsync<RelatedProductPreviewRow>(
            new CommandDefinition(sql, new { Code = relatedCode }, cancellationToken: ct));
    }

    public async Task<AddRelatedResult> AddRelatedWithStateAsync(string relatedParentCode, string relatedCode, string username, CancellationToken ct = default)
    {
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "Catalogue.usp_AddRelatedProduct";
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@ParentCode", relatedParentCode);
        cmd.Parameters.AddWithValue("@RelatedCode", relatedCode);
        cmd.Parameters.AddWithValue("@Username", username);

        var output = cmd.Parameters.Add("@State", SqlDbType.NVarChar, 20);
        output.Direction = ParameterDirection.Output;

        await cmd.ExecuteNonQueryAsync(ct);

        var state = output.Value?.ToString() ?? "";
        return new AddRelatedResult { State = state };
    }

    public async Task UnlinkByRefAsync(string productCode, string relatedCode, CancellationToken ct = default)
    {
        await using var conn = new SqlConnection(_cs);
        await conn.ExecuteAsync(new CommandDefinition(
            "Catalogue.usp_UnlinkRelatedByRef",
            new { ProductCode = productCode, RelatedCode = relatedCode },
            commandType: CommandType.StoredProcedure,
            cancellationToken: ct));
    }

    public async Task CommitRelatedChangesAsync(string productCode, string username, CancellationToken ct = default)
    {
        await using var conn = new SqlConnection(_cs);
        await conn.ExecuteAsync(new CommandDefinition(
            "Catalogue.usp_UpdateRelated",
            new { Username = username, ProductCode = productCode },
            commandType: CommandType.StoredProcedure,
            cancellationToken: ct));
    }
}