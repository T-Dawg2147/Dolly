using System.Data;
using Dapper;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dolly.Infrastructure.Services;

public sealed class ChangeCommitService(IConfiguration config) : IChangeCommitService
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
        ?? throw new InvalidOperationException("Missing connection string: CatalogueDb");

    public async Task<ApplyChangesResult> ApplyChangesAsync(string supplierCode, string productCode, string objectType, string username,
        CancellationToken ct = default)
    {
        try
        {
            await using var conn = new SqlConnection(_cs);
            await conn.OpenAsync(ct);

            await using var tx = await conn.BeginTransactionAsync(ct);

            // Builds parameters for "Catalogue.usp_UpdateStepHistory"
            var p = new DynamicParameters();
            p.Add("@SupplierCode", "%");
            p.Add("@ProductCode", productCode);
            p.Add("@Username", username);
            p.Add("@NoRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync(new CommandDefinition(
                "Catalogue.usp_UpdateStepHistory",
                p,
                transaction: tx,
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));

            await conn.ExecuteAsync(new CommandDefinition(
                "Catalogue.UpdateProductTable",
                new { Username = username },
                transaction: tx,
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));

            await conn.ExecuteAsync(new CommandDefinition(
                "Catalogue.usp_ExportToSTEP",
                new { ObjectType = objectType, Username = username },
                transaction: tx,
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));

            await tx.CommitAsync(ct);

            return new ApplyChangesResult { Success = true };
        }
        catch (Exception ex)
        {
            return new ApplyChangesResult { Success = false, ErrorMessage = ex.Message };
        }
    }
}