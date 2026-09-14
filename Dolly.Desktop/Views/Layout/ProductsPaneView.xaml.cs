using System.Windows.Controls;
using System.Windows.Input;
using Dolly.Desktop.ViewModels.Layout;

namespace Dolly.Desktop.Views.Layout;

public partial class ProductsPaneView : UserControl
{
    public ProductsPaneView()
    {
        InitializeComponent();
    }

    private async void DesignNo_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not ProductsPaneViewModel vm) return;
        if (vm.OpenSelectedProductEditCommand.CanExecute(null))
            await vm.OpenSelectedProductEditCommand.ExecuteAsync(null);
    }
}