using CommunityToolkit.Mvvm.ComponentModel;

namespace Dolly.Desktop.ViewModels.Tabs;

public partial class MainTabsHostViewModel : ObservableObject
{
    [ObservableProperty] private int selectedTabIndex;

    public SupplierDetailsViewModel SupplierDetails { get; }
    public CarriageDetailsViewModel CarriageDetails { get; }
    public PurchaseOrderDetailsViewModel PurchaseOrders { get; }
    public AdditionalInfoViewModel AdditionalInfo { get; }
    public ReportsViewModel Reports { get; }
    public QualityAuditViewModel QualityAudit { get; }
    public SalesOverviewViewModel SalesOverview { get; }
    public PageNumbersViewModel PageNumbers { get; }
    public UnspscUpdateViewModel UnspscUpdate { get; }
    public ImageUpdatesViewModel ImageUpdates { get; }

    public MainTabsHostViewModel(
        SupplierDetailsViewModel supplierDetails,
        CarriageDetailsViewModel carriageDetails,
        PurchaseOrderDetailsViewModel purchaseOrders,
        AdditionalInfoViewModel additionalInfo,
        ReportsViewModel reports,
        QualityAuditViewModel qualityAudit,
        SalesOverviewViewModel salesOverview,
        PageNumbersViewModel pageNumbers,
        UnspscUpdateViewModel unspscUpdate,
        ImageUpdatesViewModel imageUpdates)
    {
        SupplierDetails = supplierDetails;
        CarriageDetails = carriageDetails;
        PurchaseOrders = purchaseOrders;
        AdditionalInfo = additionalInfo;
        Reports = reports;
        QualityAudit = qualityAudit;
        SalesOverview = salesOverview;
        PageNumbers = pageNumbers;
        UnspscUpdate = unspscUpdate;
        ImageUpdates = imageUpdates;
    }
}