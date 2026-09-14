namespace Dolly.Application.Abstraction;

public interface IReportExportService
{
    Task ExportRunToZeroAsync(string folder, CancellationToken ct = default);
    Task ExportSupplierStockAsync(string folder, CancellationToken ct = default);
    Task ExportSupplierReportAsync(string folder, string supplierCode, bool reportingSupplier,
        CancellationToken ct = default);
    Task ExportCustomerSalesAsync(string folder, string supplierCode, bool reportingSupplier,
        CancellationToken ct = default);
    Task ExportPromoPricingAsync(string folder, CancellationToken ct = default);
    Task ExportWebRankingAsync(string folder, CancellationToken ct = default);
    Task ExportZeroSalesAsync(string folder, CancellationToken ct = default);
}