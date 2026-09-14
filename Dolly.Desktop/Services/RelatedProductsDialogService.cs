using System.Windows;
using Dolly.Desktop.ViewModels;
using Dolly.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Dolly.Desktop.Services;

public sealed class RelatedProductsDialogService(IServiceProvider sp) : IRelatedProductsDialogService
{
    public async Task ShowAsync(string productCode, string username, CancellationToken ct = default)
    {
        var vm = sp.GetRequiredService<RelatedProductsViewModel>();
        vm.Username = username;
        await vm.LoadAsync(productCode);

        var win = new RelatedProductsWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
        };

        win.ShowDialog();
    }
}