using System.Windows;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;
using Dolly.Desktop.ViewModels;
using Dolly.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Dolly.Desktop.Services;

public sealed class SearchResultsDialogService(IServiceProvider sp) : ISearchResultsDialogService
{
    public async Task<SearchNavigationTarget?> ShowAsync(string term, CancellationToken ct = default)
    {
        var vm = sp.GetRequiredService<SearchResultsViewModel>();
        SearchNavigationTarget? nav = null;

        vm.NavigateRequested += (_, target) => nav = target;
        await vm.LoadAsync(term);

        var win = new SearchResultsWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
        };

        win.ShowDialog();
        return nav;
    }
}