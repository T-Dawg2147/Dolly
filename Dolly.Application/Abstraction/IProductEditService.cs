using Dolly.Application.Models;

namespace Dolly.Application.Abstraction;

public interface IProductEditService
{
    Task<ProductEditRow?> LoadAsync(string designNo);
    Task<SaveProductEditResult> SaveAsync(ProductEditRow row);
    Task<IReadOnlyList<ProductHistoryRow>> GetHistoryAsync(string designNo);

    Task<IReadOnlyList<string>> GetSupplierNamesAsync();
    Task<IReadOnlyList<string>> GetCountryOptionsAsync();
    Task<IReadOnlyList<string>> GetWebOverlayOptionsAsync();
    Task<IReadOnlyList<string>> GetWebExclusiveOptionsAsync();
    Task<IReadOnlyList<string>> GetWebDeliveryOptionsAsync();
    Task<IReadOnlyList<string>> GetDiscontinuedReasonOptionsAsync();

    Task<RuleEvaluationResult> EvaluateRulesAsync(ProductEditRow dto);
    Task<RepriceResult> RepriceAsync(ProductEditRow dto);
    Task<OverlayUpdateResult> UpdateOverlayAsync(string designNo, string? webOverlay, string? webExclusive);
    Task<PushToMagentoResult> PushToMagentoAsync(string designNo);

    Task<ApplyChangesResult> ApplyWorkingTableAndCommitAsync(ProductEditRow dto, string username);

    Task<SaveProductEditResult> PersistPalletFieldsAsync(string designNo, bool usePalletRate, int? noOfPallets);
    
    Task<ProductEditRow?> GetProductEditAsync(string productId ,CancellationToken ct = default);
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
    Task MarkUpdateImagesAsync(string codeOrName, CancellationToken ct = default);

    Task<ProductEditComputed> LoadComputedAsync(
        string codeOrName,
        string? stockStatus,
        decimal? carriageCharge,
        decimal? currencyFactor,
        string? carriageType,
        decimal? freightOut,
        decimal? standardCost,
        decimal? weight, 
        CancellationToken ct = default);
}