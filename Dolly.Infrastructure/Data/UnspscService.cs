using Dapper;
using Dolly.Application.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dolly.Infrastructure.Data;

/// <summary>
/// MIGRATION NOTE:
/// Mirrors comUseUNSPSC_Click and supports batch mode.
/// </summary>
public sealed class UnspscService(IConfiguration config) : IUnspscService
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
        ?? throw new InvalidOperationException("Missing connection stringL CatalogueDb");

    public async Task ApplyUnspscAsync(string groupId, string segmentTitle, string commodityTitle, string commodityCode, bool batch,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new InvalidOperationException("Group Id is required.");
        if (string.IsNullOrWhiteSpace(commodityCode))
            throw new InvalidOperationException("Commodity code is required.");

        await using var conn = new SqlConnection(_cs);
        await conn.ExecuteAsync(new CommandDefinition(
            "Catalogue.usp_UpdateUNSPSC",
            new
            {
                GroupId = groupId,
                SegmentTitle = segmentTitle ?? "",
                CommodityTitle = commodityTitle ?? "",
                CommodityCode = commodityCode,
                Batch = batch ? "Yes" : "No"
            },
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: ct));
    }
}