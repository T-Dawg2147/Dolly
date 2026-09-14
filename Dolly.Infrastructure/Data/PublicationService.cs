using Dapper;
using Dolly.Application.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dolly.Infrastructure.Data;

/// <summary>
/// MIGRATION NOTE:
/// Mirrors Access comUpdateCreate_Click rules but with parameterized SQL.
/// </summary>
public sealed class PublicationService(IConfiguration config) : IPublicationService
{
    private readonly string _cs = config.GetConnectionString("CatalogueDb")
        ?? throw new InvalidOperationException("Missing connection string: CatalogueDb");

    public async Task CreateOrUpdatePublicationAsync(string publicationName, DateTime startDate, DateTime endDate, string mediaCodeUk,
        string mediaCodeRoi, string priceListType, bool isUpdate, CancellationToken ct = default)
    {
        Validate(publicationName, startDate, endDate, mediaCodeUk, priceListType);
        
        await using var conn = new SqlConnection(_cs);

        var existingUk = await conn.ExecuteScalarAsync<string?>(new CommandDefinition(
            "SELECT TOP 1 [Publication_Name] FROM Catalogue.Publication WHERE Media_Code = @Code",
            new { Code = mediaCodeUk }, cancellationToken: ct));

        if (!string.IsNullOrWhiteSpace(existingUk) &&
            !string.Equals(existingUk, publicationName, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                $"Media code '{mediaCodeUk}' already exists under another publications.");

        if (!string.IsNullOrWhiteSpace(mediaCodeRoi))
        {
            var existingRoi = await conn.ExecuteScalarAsync<string?>(new CommandDefinition(
                "SELECT TOP 1 Publication_Name FROM Catalogue.Publication WHERE Media_Code_ROI = @Code",
                new { Code = mediaCodeRoi }, cancellationToken: ct));

            if (!string.IsNullOrWhiteSpace(existingRoi) &&
                !string.Equals(existingRoi, publicationName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"ROI media code '{mediaCodeRoi}' already exists under another publication.");
        }

        await conn.ExecuteAsync(new CommandDefinition(
            "Catalogue.usp_CreatePublication",
            new
            {
                PublicationName = publicationName,
                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd"),
                MediaCodeUk = mediaCodeUk,
                MediaCodeRoi = mediaCodeRoi,
                PriceListType = priceListType,
                isUpdate = isUpdate ? "Yes" : "No"
            },
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: ct));
    }

    private void Validate(
        string publicationName,
        DateTime startDate,
        DateTime endDate,
        string mediaCodeUk,
        string priceListType)
    {
        if (string.IsNullOrWhiteSpace(publicationName))
            throw new InvalidOperationException("Publication name is required.");
        if (string.IsNullOrWhiteSpace(mediaCodeUk))
            throw new InvalidOperationException("UK media code is required.");
        if (string.IsNullOrWhiteSpace(priceListType))
            throw new InvalidOperationException("Price list type is required.");
        if (endDate <= startDate)
            throw new InvalidOperationException("End date must be after start date.");
    }
}