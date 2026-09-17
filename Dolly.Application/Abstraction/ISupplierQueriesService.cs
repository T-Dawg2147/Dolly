using Dolly.Application.Models;

namespace Dolly.Application.Abstraction;

public interface ISupplierQueriesService
{
    Task<IReadOnlyList<SupplierSummary>> SearchSuppliersAsync(string term, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetCountries(CancellationToken ct = default);
    Task<SupplierDetailsRow?> GetSupplierDetailsAsync(string supplierCode, CancellationToken ct = default);
    Task<IReadOnlyList<SupplierProductRow>> GetSupplierProductsAsync(string supplierCode, int productStatus,
        CancellationToken ct = default);
    Task<IReadOnlyList<SupplierProductSalesRow>> GetSupplierProductSalesAsync(string supplierCode, int status,
        CancellationToken ct = default);
    Task<ProductOverviewRow?> GetProductOverviewAsync(string code, CancellationToken ct = default);
    Task SaveSupplierAddressAsync(string supplierCode, string username, CancellationToken ct = default);
}