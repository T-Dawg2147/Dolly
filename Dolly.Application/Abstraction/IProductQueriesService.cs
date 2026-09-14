using Dolly.Application.Models;

namespace Dolly.Application.Abstraction;

public interface IProductQueriesService
{
    /*Task<ProductEditRow?> GetProductEditAsync(string productId ,CancellationToken ct = default);
    Task<IReadOnlyList<ProductHistoryRow>> GetProductHistoryAsync(string productIdOrName, CancellationToken ct = default);
    Task<ProductCostingSnapshotRow?> RecalculateCostingAsync(
        decimal? carriageCharge,
        decimal? currencyFactor,
        string? carriageType,
        decimal? freightOut,
        decimal? standardCost,
        string? stockStatus,
        decimal? weight,
        string? codeOrName,
        CancellationToken ct = default);

    Task<string?> GetProductImagePathAsync(string codeOrName, CancellationToken ct = default);

    Task<ProductAuxLookups> GetAuxLookupsAsync(string codeOrName, string? stockStatus, CancellationToken ct = default);
    Task<bool> HasRelatedProductsAsync(string codeOrName, CancellationToken ct = default);
    Task<bool> HasDualFeatureChildrenAsync(string codeOrName, CancellationToken ct = default);
    Task MarkUpdateImagesAsync(string codeOrName, CancellationToken ct = default);*/
}