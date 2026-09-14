using System.Windows;
using Dolly.Application.Abstraction;
using Dolly.Desktop.ViewModels;
using Dolly.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Dolly.Desktop.Services;

public sealed class AddRelatedProductDialogService(IServiceProvider sp) : IAddRelatedProductDialogService
{
    public async Task<bool> ShowAsync(string parentCode, CancellationToken ct = default)
    {
        var vm = sp.GetRequiredService<AddRelatedProductViewModel>();
        var added = false;

        vm.AddedSuccessfully += (_, _) => added = true;
        await vm.LoadAsync(parentCode);

        var win = new AddRelatedProductWindow
        {
            DataContext = vm,
            Owner = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
        };

        vm.RequestClose += (_, _) => win.Close();
        win.ShowDialog();

        return added;
    }
}