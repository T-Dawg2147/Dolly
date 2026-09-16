using System.Windows;
using System.Windows.Controls;
using Dolly.Desktop.Utilities;

namespace Dolly.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(SuppliersAndProductsView view)
    {
        InitializeComponent();
        ViewHost.Content = view;

        this.SizeChanged += MainWindow_SizeChanged;
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"[MainWindow] Resized to {e.NewSize.Width} x {e.NewSize.Height}");
    }
}