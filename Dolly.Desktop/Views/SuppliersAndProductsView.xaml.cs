using System.Windows.Controls;
using Dolly.Desktop.ViewModels.Layout;

namespace Dolly.Desktop.Views;

public partial class SuppliersAndProductsView : UserControl
{
    public SuppliersAndProductsView(SuppliersAndProductsShellViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}