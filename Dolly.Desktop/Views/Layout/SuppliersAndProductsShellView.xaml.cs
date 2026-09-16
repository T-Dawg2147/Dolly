using System.Windows;
using System.Windows.Controls;
using Dolly.Desktop.Utilities;

namespace Dolly.Desktop.Views.Layout;

public partial class SuppliersAndProductsShellView : UserControl
{
    private LayoutManager _layoutManager;
    
    public SuppliersAndProductsShellView()
    {
        InitializeComponent();
        Loaded += OnViewLoaded;
    }
    
    private void OnViewLoaded(object sender, RoutedEventArgs e)
    {
        // Initialize the layout manager with the named UI elements
        _layoutManager = new LayoutManager(
            productImageBorder: ProductImageBorder,
            productOverviewBorder: ProductOverviewBorder,
            mainGridRow0: TopPanelsGrid,
            productsPaneBorder: ProductsPaneBorder
        );

        // Subscribe to size changed events on the parent window
        if (Window.GetWindow(this) is Window window)
        {
            window.SizeChanged += Window_SizeChanged;
            
            // Perform initial layout calculation
            System.Diagnostics.Debug.WriteLine($"Initial window size: {window.ActualWidth} x {window.ActualHeight}");
            _layoutManager.OnWindowSizeChanged(window.ActualWidth, window.ActualHeight);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Could not find parent window!");
        }
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Window resized to: {e.NewSize.Width} x {e.NewSize.Height}");
        _layoutManager?.OnWindowSizeChanged(e.NewSize.Width, e.NewSize.Height);
    }
}