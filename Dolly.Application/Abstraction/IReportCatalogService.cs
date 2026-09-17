using Dolly.Application.Models;

namespace Dolly.Application.Abstraction;

public interface IReportCatalogService
{
    Task<IReadOnlyList<ReportDefinition>> GetReportsAsync(CancellationToken ct = default);
}