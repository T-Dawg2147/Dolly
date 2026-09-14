using System.Windows;
using Dolly.Desktop.ViewModels;

namespace Dolly.Desktop.Views;

public partial class RelatedProductsWindow : Window
{
    public RelatedProductsWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not RelatedProductsViewModel vm) return;
        vm.RequestClose += (_, _) => Close();
    }
}