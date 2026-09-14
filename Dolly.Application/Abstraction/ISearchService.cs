using Dolly.Application.Models;

namespace Dolly.Application.Abstraction;

public interface ISearchService
{
    Task<IReadOnlyList<SearchResultRow>> SearchAsync(string searchTerm, CancellationToken ct = default);

    Task<SearchNavigationTarget?> ResolveNavigationTargetAsync(SearchResultRow row, bool reportingMode,
        CancellationToken ct = default);
}