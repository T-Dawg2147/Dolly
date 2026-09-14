using System.Windows;
using Dolly.Desktop.Utilities;

namespace Dolly.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly LayoutManager _layoutManager;
    
    public MainWindow(SuppliersAndProductsView view)
    {
        InitializeComponent();
        ViewHost.Content = view;
        Loaded += OnViewLoaded;
    }

    private void OnViewLoaded(object sender, RoutedEventArgs e)
    {
        
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _layoutManager.OnWindowSizeChanged(e.NewSize.Width, e.NewSize.Height);
    }
}