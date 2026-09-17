using Dolly.Application.Models;

namespace Dolly.Application.Abstraction;

public interface IGenericReportRunner
{
    Task RunAsync(ReportDefinition report, string outputFolder, string? supplierCode, bool reportingSupplier,
        IReadOnlyDictionary<string, object?>? extraParameters = null, CancellationToken ct = default);
}