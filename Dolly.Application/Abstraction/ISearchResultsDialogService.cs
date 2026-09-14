using Dolly.Application.Models;

namespace Dolly.Application.Abstraction;

public interface ISearchResultsDialogService
{
    Task<SearchNavigationTarget?> ShowAsync(string term, CancellationToken ct = default);
}