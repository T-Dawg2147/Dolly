using System.Windows;
using System.Windows.Controls;
using Dolly.Application.Abstraction;

namespace Dolly.Desktop.Utilities;

/// <summary>
/// Manages responsive layout behaviour for the SuppliersAndProductsView.
/// Handles visibility and sizing of ProductImageView and ProductOverview based on window dimensions.
/// </summary>
public class LayoutManager : ILayoutManager
{
    private const double FirstBreakpointWidth = 21165 / 20.0;          // 1058.25 DIU - Show image
    private const double SecondBreakpointWidth = 16460 / 20.0;         // 823 DIU - Minimum width for layout
    private const double ThirdBreakpointWidth = 26360 / 20.0;          // 1318 DIU - Show product overview
    private const double MinimumHeightForTabControl = 6270 / 20.0;     // 313.5 DIU

    // Converted dimension constants
    private const double ProductImageWidth = 4345 / 20.0;              // 217.25 DIU
    private const double WidthAdjustment = 583 / 20.0;                 // 29.15 DIU
    private const double DoubleWidthAdjustment = 1166 / 20.0;          // 58.3 DIU
    private const double ProductOverviewWidth = 6520 / 20.0;           // 326 DIU
    private const double ProductOverviewLeftPosition = 20948 / 20.0;   // 1047.4 DIU
    private const double TabControlHeightAdjustment = 6853 / 20.0;     // 342.65 DIU
    private const double ProductOverviewLeftAlt = 16287 / 20.0;        // 814.35 DIU

    private readonly UIElement _productImageView;
    private readonly UIElement _productOverviewView;
    private readonly FrameworkElement _supplierDetailsControl;
    private readonly FrameworkElement _tabControlProducts;
    private readonly FrameworkElement _subSupplierProducts;
    private readonly FrameworkElement _subSupplierProductSales;

    public LayoutManager(
        UIElement productImageView,
        UIElement productOverviewView,
        FrameworkElement supplierDetailsControl,
        FrameworkElement tabControlProducts,
        FrameworkElement subSupplierProducts,
        FrameworkElement subSupplierProductSales)
    {
        _productImageView = productImageView;
        _productOverviewView = productOverviewView;
        _supplierDetailsControl = supplierDetailsControl;
        _tabControlProducts = tabControlProducts;
        _subSupplierProducts = subSupplierProducts;
        _subSupplierProductSales = subSupplierProductSales;
    }

    public void OnWindowSizeChanged(double height, double width)
    {
        try
        {
            HandleProductImageVisibility(width);
            HandleProductOverviewVisibility(width);
            HandleTabControlHeight(height, width);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "There was an error when trying to resize the form.",
                "Resize Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    /// <summary>
    /// Controls the visibility and width of the ProductImageView based on window width.
    /// </summary>
    private void HandleProductImageVisibility(double windowWidth)
    {
        if (windowWidth >= FirstBreakpointWidth)
        {
            // Show product image when window is wide enough
            _productImageView.Visibility = Visibility.Visible;
            if (_productImageView is FrameworkElement imageElement)
            {
                imageElement.Width = ProductImageWidth;
            }

            UpdateControlWidths(windowWidth);
        }
        else
        {
            // Hide product image when window is too narrow
            _productImageView.Visibility = Visibility.Collapsed;
            if (_productImageView is FrameworkElement imageElement)
            {
                imageElement.Width = 0;
            }

            // Still update widths if minimum width is met
            if (windowWidth - WidthAdjustment > SecondBreakpointWidth)
            {
                UpdateControlWidths(windowWidth);
            }
        }
    }
    
    /// <summary>
    /// Updates the width of supplier details and tab controls.
    /// </summary>
    private void UpdateControlWidths(double windowWidth)
    {
        double adjustedWidth = windowWidth - WidthAdjustment;
        _supplierDetailsControl.Width = adjustedWidth;
        _tabControlProducts.Width = adjustedWidth;
        _subSupplierProducts.Width = windowWidth - DoubleWidthAdjustment;
        _subSupplierProductSales.Width = windowWidth - DoubleWidthAdjustment;
    }

    /// <summary>
    /// Controls the visibility and position of the ProductOverviewView based on window width.
    /// </summary>
    private void HandleProductOverviewVisibility(double windowWidth)
    {
        var overview = _productOverviewView as FrameworkElement;
        if (overview == null) return;

        if (windowWidth >= ThirdBreakpointWidth)
        {
            // Show product overview on the right side
            if (overview is Grid overviewGrid)
            {
                Grid.SetColumn(overviewGrid, 2); // Position in right column if using Grid
            }

            overview.Visibility = Visibility.Visible;
            overview.Width = ProductOverviewWidth;

            // Set left position if it's a control that supports it
            if (overview is Control)
            {
                overview.Margin = new Thickness(ProductOverviewLeftPosition, 0, 0, 0);
            }
        }
        else
        {
            // Hide product overview
            overview.Visibility = Visibility.Collapsed;
            overview.Width = 0;
            overview.Margin = new Thickness(ProductOverviewLeftAlt, 0, 0, 0);
        }
    }

    /// <summary>
    /// Adjusts the height of tab controls based on window height.
    /// </summary>
    private void HandleTabControlHeight(double windowHeight, double windowWidth)
    {
        if (windowHeight > MinimumHeightForTabControl)
        {
            double adjustedHeight = windowHeight - TabControlHeightAdjustment;
            _tabControlProducts.Height = adjustedHeight;
            _subSupplierProducts.Height = adjustedHeight;
            _subSupplierProductSales.Height = adjustedHeight;
        }
    }
}