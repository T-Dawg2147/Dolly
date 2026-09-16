using System.Windows;
using System.Windows.Controls;

namespace Dolly.Desktop.Utilities;

/// <summary>
/// Manages responsive layout behavior for the SuppliersAndProductsShellView.
/// Handles visibility and sizing of ProductImageView and ProductOverviewView based on window dimensions.
/// </summary>
public class LayoutManager
{
    // Breakpoint constants (in DIUs converted from the original twips values)
    private const double FirstBreakpointWidth = 1205;          // 1058.25 DIU - Show image
    private const double SecondBreakpointWidth = 16460 / 20.0;         // 823 DIU - Minimum width for layout
    private const double ThirdBreakpointWidth = 1300;          // 1318 DIU - Show product overview

    // Converted dimension constants
    private const double ProductImageWidth = 360;              // 217.25 DIU
    private const double WidthAdjustment = 583 / 20.0;                 // 29.15 DIU
    private const double DoubleWidthAdjustment = 1166 / 20.0;          // 58.3 DIU
    private const double ProductOverviewWidth = 350;           // 326 DIU
    private const double ProductOverviewLeftPosition = 20948 / 20.0;   // 1047.4 DIU

    private readonly FrameworkElement _productImageBorder;
    private readonly FrameworkElement _productOverviewBorder;
    private readonly FrameworkElement _mainGridRow0;
    private readonly FrameworkElement _productsPaneBorder;

    /// <summary>
    /// Initializes a new instance of the ResponsiveLayoutManager.
    /// </summary>
    public LayoutManager(
        FrameworkElement productImageBorder,
        FrameworkElement productOverviewBorder,
        FrameworkElement mainGridRow0,
        FrameworkElement productsPaneBorder)
    {
        _productImageBorder = productImageBorder;
        _productOverviewBorder = productOverviewBorder;
        _mainGridRow0 = mainGridRow0;
        _productsPaneBorder = productsPaneBorder;
    }

    /// <summary>
    /// Handles window resize event and updates layout accordingly.
    /// </summary>
    public void OnWindowSizeChanged(double windowWidth, double windowHeight)
    {
        try
        {
            HandleProductImageVisibility(windowWidth);
            HandleProductOverviewVisibility(windowWidth);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"There was an error when trying to resize the form: {ex.Message}",
                "Resize Error",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

    private void HandleProductImageVisibility(double windowWidth)
    {
        if (windowWidth >= FirstBreakpointWidth)
        {
            System.Diagnostics.Debug.WriteLine($"[LayoutManager] Showing ProductImage (width {windowWidth} >= {FirstBreakpointWidth})");
            // Show product image when window is wide enough
            _productImageBorder.Visibility = Visibility.Visible;
            _productImageBorder.Width = ProductImageWidth;
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[LayoutManager] Hiding ProductImage (width {windowWidth} < {FirstBreakpointWidth})");
            // Hide product image when window is too narrow
            _productImageBorder.Visibility = Visibility.Collapsed;
            _productImageBorder.Width = 0;
        }
    }

    private void HandleProductOverviewVisibility(double windowWidth)
    {
        if (windowWidth >= ThirdBreakpointWidth)
        {
            System.Diagnostics.Debug.WriteLine($"[LayoutManager] Showing ProductOverview (width {windowWidth} >= {ThirdBreakpointWidth})");
            // Show product overview on the right side
            _productOverviewBorder.Visibility = Visibility.Visible;
            _productOverviewBorder.Width = ProductOverviewWidth;
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[LayoutManager] Hiding ProductOverview (width {windowWidth} < {ThirdBreakpointWidth})");
            // Hide product overview
            _productOverviewBorder.Visibility = Visibility.Collapsed;
            _productOverviewBorder.Width = 0;
        }
    }
}