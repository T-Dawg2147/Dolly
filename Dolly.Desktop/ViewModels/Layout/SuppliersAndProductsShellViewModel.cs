using CommunityToolkit.Mvvm.ComponentModel;
using Dolly.Desktop.ViewModels.Tabs;

namespace Dolly.Desktop.ViewModels.Layout;

public partial class SuppliersAndProductsShellViewModel : ObservableObject
{
    public MainTabsHostViewModel TabsHost { get; }
    public ProductsPaneViewModel ProductsPane { get; }
    public ProductImageViewModel ProductImage { get; }
    public ProductOverviewPanelViewModel ProductOverview { get; }

    public SuppliersAndProductsShellViewModel(
        MainTabsHostViewModel tabsHost,
        ProductsPaneViewModel productsPane,
        ProductImageViewModel productImage,
        ProductOverviewPanelViewModel productOverview)
    {
        TabsHost = tabsHost;
        ProductsPane = productsPane;
        ProductImage = productImage;
        ProductOverview = productOverview;
    }
}