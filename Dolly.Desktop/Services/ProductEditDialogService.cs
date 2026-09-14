using System.Windows;
using Dolly.Application.Abstraction;
using Dolly.Desktop.ViewModels;
using Dolly.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Dolly.Desktop.Services;

public sealed class ProductEditDialogService(IServiceProvider sp) : IProductEditDialogService
{
    public async Task<bool?> ShowAsync(string productId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var vm = ActivatorUtilities.CreateInstance<ProductEditViewModel>(sp, productId);

        await vm.InitializeAsync();

        var owner = System.Windows.Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsActive);

        var window = new ProductEditWindow
        {
            DataContext = vm,
            Owner = owner,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        void Handler(object? _, bool saved)
        {
            window.DialogResult = saved;
            window.Close();
        }

        vm.RequestClose += Handler;
        try
        {
            return window.ShowDialog();
        }
        finally
        {
            vm.RequestClose -= Handler;
        }
    }
}