using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;

namespace Dolly.Desktop.ViewModels;

public partial class ProductEditViewModel : ObservableObject, IDisposable
{
    private readonly IProductEditService _service;
    private readonly string _designNoArg;
    private readonly IUserDialogService _dialogs;
    private readonly SupplierContextState _context;
    private readonly SemaphoreSlim _recomputeGate = new(1, 1);
    
    private bool _isInitializing;
    private int _recomputeVersion;
    
    public event EventHandler<bool>? RequestClose;

    #region Observable fields
    // ---------- Runtime helpers ----------
    [ObservableProperty] private string _windowTitle = "Product Edit";
    [ObservableProperty] private string _username = "";
    [ObservableProperty] private bool _isDirty;
    [ObservableProperty] private bool _isDiscontinuedReasonVisible;
    [ObservableProperty] private bool _hasInvalidPriceBreak;
    [ObservableProperty] private bool _netCost2Invalid;
    [ObservableProperty] private bool _netCost3Invalid;
    [ObservableProperty] private bool _minQty2Invalid;
    [ObservableProperty] private bool _minQty3Invalid;
    [ObservableProperty] private bool _isDualFeatureLocked;
    [ObservableProperty] private string? _palletWarning;

    // ---------- Persisted: header/details ----------
    [ObservableProperty] private string? _id;
    [ObservableProperty] private string? _designNo;
    [ObservableProperty] private int? _pageNo;
    [ObservableProperty] private string? _objectType;
    [ObservableProperty] private string? _baseCode;
    [ObservableProperty] private string? _longDescription1;
    [ObservableProperty] private string? _longDescription2;
    [ObservableProperty] private string? _parentId;

    // ---------- Persisted: supplier/cost input ----------
    [ObservableProperty] private string? _supplierName; // runtime/display if no DB column
    [ObservableProperty] private string? _supplierCode;
    [ObservableProperty] private string? _supplierProductCode;
    [ObservableProperty] private decimal? _supplierListPrice;
    [ObservableProperty] private decimal? _supplierDiscount;
    [ObservableProperty] private decimal? _netCost2;
    [ObservableProperty] private decimal? _netCost3;
    [ObservableProperty] private decimal? _currencyFactor;
    [ObservableProperty] private decimal? _freightIn;
    [ObservableProperty] private decimal? _freightOut;
    [ObservableProperty] private string? _carriageType;
    [ObservableProperty] private decimal? _carriageCharge;
    [ObservableProperty] private decimal? _actualLandedCost;
    [ObservableProperty] private int? _minQty2;
    [ObservableProperty] private int? _minQty3;
    [ObservableProperty] private decimal? _additionalCosts;

    // ---------- Calculated ----------
    [ObservableProperty] private decimal? _netCost;
    [ObservableProperty] private decimal? _standardCost;
    [ObservableProperty] private decimal? _totalCost;
    [ObservableProperty] private decimal? _margin1;
    [ObservableProperty] private decimal? _margin2;
    [ObservableProperty] private decimal? _margin3;
    [ObservableProperty] private decimal? _soMargin;

    // ---------- Persisted: carriage/zones ----------
    [ObservableProperty] private decimal? _zoneA;
    [ObservableProperty] private decimal? _zoneB;
    [ObservableProperty] private decimal? _zoneC;
    [ObservableProperty] private decimal? _zoneD;
    [ObservableProperty] private decimal? _zoneE;
    [ObservableProperty] private decimal? _zoneS;
    [ObservableProperty] private string? _shippingMethod;
    [ObservableProperty] private decimal? _shippingCost;

    // ---------- Unbound (runtime/calculated sources) ----------
    [ObservableProperty] private string? _zoneDefinitions;
    [ObservableProperty] private decimal? _smallOrderValue;
    [ObservableProperty] private decimal? _smallOrderCharge;
    [ObservableProperty] private string? _despatchMethod;
    [ObservableProperty] private decimal? _despatchCost;

    // ---------- Persisted: selling ----------
    [ObservableProperty] private decimal? _sellingPrice1;
    [ObservableProperty] private decimal? _sellingPrice2;
    [ObservableProperty] private decimal? _sellingPrice3;
    [ObservableProperty] private decimal? _ukPromoPrice;
    [ObservableProperty] private decimal? _roiPromoPrice;

    // ---------- Persisted + flag/raw ----------
    [ObservableProperty] private string? _stockStatus;
    [ObservableProperty] private string? _greenFlag;
    [ObservableProperty] private string? _countryOfOrigin;
    [ObservableProperty] private int? _leadTime;
    [ObservableProperty] private DateTime? _supplierDueDate;
    [ObservableProperty] private string? _webDeliveryIcon;
    [ObservableProperty] private int? _minOrderQuantity;
    [ObservableProperty] private string? _tariffCode;
    [ObservableProperty] private string? _manageStock;
    [ObservableProperty] private string? _component;
    [ObservableProperty] private string _madeToOrder;
    [ObservableProperty] private string _madeInUk;

    // ---------- Persisted: volumetrics ----------
    [ObservableProperty] private decimal? _packagingHeight;
    [ObservableProperty] private decimal? _packagingWidth;
    [ObservableProperty] private decimal? _packagingLength;
    [ObservableProperty] private decimal? _weight;
    [ObservableProperty] private int? _noOfPallets;

    // ---------- Persisted ----------
    [ObservableProperty] private DateTime? _discontinuedDate;
    [ObservableProperty] private string? _discontinuedReason;
    [ObservableProperty] private DateTime? _releaseDate;
    [ObservableProperty] private string? _webStores;
    [ObservableProperty] private string? _productSupplierMessage;
    [ObservableProperty] private string? _productMessage;
    [ObservableProperty] private DateTime? _messageStartDate;
    [ObservableProperty] private DateTime? _messageEndDate;
    [ObservableProperty] private string? _nonTransactionalText;

    // ---------- Runtime/media ----------
    [ObservableProperty] private string? _primaryImagePath;

    // ---------- Persisted overlays ----------
    [ObservableProperty] private string? _webOverlay;
    [ObservableProperty] private string? _webExclusive;

    // ---------- Bool fields (VM bool ↔ DB string flags) ----------
    [ObservableProperty] private bool _useActualCost;      // UI-only rule toggle
    [ObservableProperty] private bool _usePalletRate;
    [ObservableProperty] private bool _boxed;
    [ObservableProperty] private bool _runToZero;
    [ObservableProperty] private bool _transactional;
    [ObservableProperty] private bool _showRelatedOnWeb;
    #endregion

    public ObservableCollection<string> SupplierNameOptions { get; } = [];
    public ObservableCollection<string> StockStatusOptions { get; } = [];
    public ObservableCollection<string> CountryOptions { get; } = [];
    public ObservableCollection<string> WebDeliveryOptions { get; } = [];
    public ObservableCollection<string> WebStoreOptions { get; } = [];
    public ObservableCollection<string> WebOverlayOptions { get; } = [];
    public ObservableCollection<string> WebExclusiveOptions { get; } = [];
    public ObservableCollection<string> DiscontinuedReasonOptions { get; } = [];
    public ObservableCollection<string> YesNoOptions { get; } = [];

    public ObservableCollection<ProductHistoryRow> ProductHistoryRows { get; } = []; //TODO: Setup double click opens full history details

    public ProductEditViewModel(IProductEditService service, string designNo, IUserDialogService dialogs, SupplierContextState context)
    {
        _service = service;
        _designNoArg = designNo;
        _dialogs = dialogs;
        _context = context;
    }

    public async Task InitializeAsync()
    {
        _isInitializing = true;
        try
        {
            await LoadLookupsAsync();
            if (string.IsNullOrWhiteSpace(_designNoArg)) return;
            
            var row = await _service.LoadAsync(_designNoArg);
            if (row is null) return;

            MapFromRow(row);

            await LoadComputedAsync();
            await LoadImageAsync();
            await LoadHistoryAsync();

            ApplyUiRules();
            await CheckPriceBreaksAsync();
        }
        finally
        {
            _isInitializing = false;
            IsDirty = false;
        }
    }

    private async Task LoadComputedAsync()
    {
        if (string.IsNullOrWhiteSpace(DesignNo)) return;

        var c = await _service.LoadComputedAsync(
            DesignNo, StockStatus, CarriageCharge, CurrencyFactor, CarriageType, FreightOut, StandardCost, Weight);

        BaseCode = c.BaseCode;
        DespatchMethod = c.DespatchMethod;
        DespatchCost = c.DespatchCost;
        SmallOrderValue = c.SmallOrderValue;
        SmallOrderCharge = c.SmallOrderCharge;
        UkPromoPrice = c.UkPromoPrice;
        RoiPromoPrice = c.RoiPromoPrice;
        WindowTitle = string.IsNullOrWhiteSpace(c.LocationPath) ? "Product Edit" : c.LocationPath;

        NetCost = CalcNetCost(SupplierListPrice, SupplierDiscount);
        StandardCost = CalcStandardCost(NetCost, CurrencyFactor, FreightIn, StockStatus);
        TotalCost = CalcTotalCost(UseActualCost, ActualLandedCost, StandardCost, ShippingCost, AdditionalCosts);

        Margin1 = CalcMargin(SellingPrice1, TotalCost);
        Margin2 = CalcMargin(SellingPrice2, (NetCost2 ?? 0) > 0 ? NetCost2 : TotalCost);
        Margin3 = CalcMargin(SellingPrice3, (NetCost3 ?? 0) > 0 ? NetCost3 : TotalCost);

        SoMargin = CalcMargin(SellingPrice1, (TotalCost ?? 0) + (SmallOrderCharge ?? 0));
    }

    private async Task LoadComputedAsyncGuarded(int version)
    {
        await _recomputeGate.WaitAsync();
        try
        {
            if (version != _recomputeVersion) return;

            await LoadComputedAsync();

            if (version != _recomputeVersion) return;
        }
        finally
        {
            _recomputeGate.Release();
        }
    }

    private async Task LoadImageAsync()
    {
        if (string.IsNullOrWhiteSpace(DesignNo)) return;
        PrimaryImagePath = await _service.GetProductImagePathAsync(DesignNo);
    }

    private async Task LoadHistoryAsync()
    {
        if (string.IsNullOrWhiteSpace(_designNoArg)) return;
        
        ProductHistoryRows.Clear();
        var history = await _service.GetHistoryAsync(_designNoArg);
        foreach (var h in history.OrderBy(h => h.ChangedDate)) ProductHistoryRows.Add(h);
    }

    private void ApplyUiRules()
    {
        var objectType = ObjectType ?? string.Empty;
        var isWithdrawn = objectType.Contains("withdrawn", StringComparison.OrdinalIgnoreCase);
        var isDual = objectType.Contains("dual", StringComparison.OrdinalIgnoreCase);

        IsDualFeatureLocked = isDual;
        
        IsDiscontinuedReasonVisible = isWithdrawn && string.IsNullOrWhiteSpace(DiscontinuedReason);

        ShowRelatedOnWeb = ShowRelatedOnWeb;
        RunToZero = RunToZero;
        UsePalletRate = UsePalletRate;

        if (Transactional)
        {
            if (!string.IsNullOrWhiteSpace(NonTransactionalText))
                NonTransactionalText = string.Empty;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(NonTransactionalText))
            {
                NonTransactionalText =
                    "Unfortunately this product is not currently available at this moment in time, however we may have a suitable alternative that would meet your requirements elsewhere on the site. " +
                    "Should you not be able to find one, or if you would like us to assist in finding one for you then please complete the form below and one of our sales team will get back in touch with you soon.";
            }
        }

        PalletWarning = null;
        if (UsePalletRate)
        {
            if (!string.Equals(StockStatus, "Stocked", StringComparison.OrdinalIgnoreCase))
                PalletWarning = "Pallet rates should only be applied to stocked lines.";
            if ((NoOfPallets ?? 0) == 0)
                PalletWarning = "Pallet rate enabled but number of pallets is 0.";
        }

        if (!string.Equals(StockStatus, "Stocked", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(GreenFlag, "Yes", StringComparison.OrdinalIgnoreCase))
        {
            // TODO: 
        }
    }

    #region Partial UI Update methods
    
    // --- Recompute pipeline triggers ---
    partial void OnSupplierListPriceChanged(decimal? value) => TriggerRecompute();
    partial void OnSupplierDiscountChanged(decimal? value) => TriggerRecompute();
    partial void OnCurrencyFactorChanged(decimal? value) => TriggerRecompute();

    partial void OnFreightInChanged(decimal? value) => TriggerRecompute();
    partial void OnFreightOutChanged(decimal? value) => TriggerRecompute();

    partial void OnCarriageTypeChanged(string? value) => TriggerRecompute();
    partial void OnCarriageChargeChanged(decimal? value) => TriggerRecompute();

    partial void OnStockStatusChanged(string? value)
    {
        TriggerRecompute();
        ApplyUiRules(); // stock-status dependent UI rules
    }

    partial void OnWeightChanged(decimal? value) => TriggerRecompute();
    partial void OnUseActualCostChanged(bool value) => TriggerRecompute();
    partial void OnActualLandedCostChanged(decimal? value) => TriggerRecompute();
    partial void OnAdditionalCostsChanged(decimal? value) => TriggerRecompute();

    partial void OnSellingPrice1Changed(decimal? value) => TriggerRecompute();
    partial void OnSellingPrice2Changed(decimal? value) => TriggerRecompute();
    partial void OnSellingPrice3Changed(decimal? value) => TriggerRecompute();

    partial void OnNetCost2Changed(decimal? value)
    {
        _ = CheckPriceBreaksAsync();
        TriggerRecompute();
    }

    partial void OnNetCost3Changed(decimal? value)
    {
        _ = CheckPriceBreaksAsync();
        TriggerRecompute();
    }

    partial void OnMinQty2Changed(int? value) => _ = CheckPriceBreaksAsync();
    partial void OnMinQty3Changed(int? value) => _ = CheckPriceBreaksAsync();
    partial void OnNetCostChanged(decimal? value) => _ = CheckPriceBreaksAsync();
    
    partial void OnMadeToOrderChanged(string? value) => MarkDirty();
    partial void OnMadeInUkChanged(string? value) => MarkDirty();

    // --- Bool/flag UI behavior ---
    partial void OnUsePalletRateChanged(bool value)
    {
        if (!value) NoOfPallets = 0;
        ApplyUiRules();
        MarkDirty();
    }

    partial void OnNoOfPalletsChanged(int? value)
    {
        ApplyUiRules();
        MarkDirty();
    }

    partial void OnBoxedChanged(bool value) => MarkDirty();

    partial void OnRunToZeroChanged(bool value)
    {
        // optional: enforce stocked-only rule here or in command
        ApplyUiRules();
        MarkDirty();
    }

    partial void OnTransactionalChanged(bool value)
    {
        ApplyUiRules();
        MarkDirty();
    }

    partial void OnShowRelatedOnWebChanged(bool value) => MarkDirty();

    partial void OnObjectTypeChanged(string? value)
    {
        ApplyUiRules();
        MarkDirty();
    }

    partial void OnDiscontinuedReasonChanged(string? value)
    {
        ApplyUiRules();
        MarkDirty();
    }

    protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (_isInitializing) return;
        if (e.PropertyName is nameof(IsDirty) or nameof(HasInvalidPriceBreak)) return;
        IsDirty = true;
    }
    #endregion

    private void MarkDirty()
    {
        if (!_isInitializing) IsDirty = true;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await CheckPriceBreaksAsync();

        if (IsDiscontinuedReasonVisible && string.IsNullOrWhiteSpace(DiscontinuedReason))
        {
            await _dialogs.ShowErrorAsync("You must enter a reason for discontinuing this product.", "Enter reason");
            return;
        }

        if (!string.IsNullOrWhiteSpace(ProductMessage) && !MessageStartDate.HasValue)
        {
            await _dialogs.ShowErrorAsync("Please enter a start date for the product message.", "Enter a start date");
            return;
        }

        if (HasInvalidPriceBreak)
        {
            await _dialogs.ShowErrorAsync("Incorrect cost price breaks. Please correct and save again.", "Incorrect Cost Price Breaks");
            return;
        }

        if (UsePalletRate)
        {
            var pallets = NoOfPallets ?? 0;
            if (pallets == 0)
            {
                await _dialogs.ShowErrorAsync("You must enter number of pallets.", "Please enter a number");
                return;
            }

            if (pallets > 3)
            {
                var yes = await _dialogs.ConfirmYesNoAsync($"Are you sure this will use {pallets} pallets to ship?", "Please check quantity");
                if (!yes)
                {
                    NoOfPallets = 0;
                    return;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(DesignNo))
        {
            var pallet = await _service.PersistPalletFieldsAsync(DesignNo, UsePalletRate, NoOfPallets);
            if (!pallet.Success)
            {
                await _dialogs.ShowErrorAsync(pallet.ErrorMessage ?? "Could not persist pallet fields.", "Save error");
                return;
            }
        }

        if (IsDirty)
        {
            var confirm = await _dialogs.ConfirmYesNoAsync("Changes have been made, do you wish to save them?", "Save Changes");
            if (!confirm) return;
        }

        var row = ToRow();
        var save = await _service.SaveAsync(row);
        if (!save.Success)
        {
            await _dialogs.ShowErrorAsync(save.ErrorMessage ?? "Failed to save product.", "Save error");
            return;
        }

        var apply = await _service.ApplyWorkingTableAndCommitAsync(row, Username);
        if (!apply.Success)
        {
            await _dialogs.ShowErrorAsync(apply.ErrorMessage ?? "Failed to apply/commit changes.", "Commit error");
            return;
        }

        IsDirty = false;
        CloseWithResult(true);
    }

    [RelayCommand] private Task CancelAsync() { CloseWithResult(false); return Task.CompletedTask; }

    [RelayCommand]
    private async Task RePriceAsync()
    {
        var result = await _service.RepriceAsync(ToRow());
        if (!result.Success) return;

        ShippingCost = result.NewDespatchCost ?? ShippingCost;
        TotalCost = result.NewTotalCost ?? TotalCost;
        SellingPrice1 = result.NewSellingPrice1 ?? SellingPrice1;
        Margin1 = result.NewMargin ?? Margin1; // TODO: Margin is calculated
    }

    [RelayCommand]
    private async Task UpdateOverlayAsync()
    {
        if (string.IsNullOrWhiteSpace(DesignNo)) return;
        await _service.UpdateOverlayAsync(DesignNo, WebOverlay, WebExclusive);
    }

    [RelayCommand]
    private async Task PushMagentoAsync()
    {
        if (string.IsNullOrWhiteSpace(DesignNo)) return;
        await _service.PushToMagentoAsync(DesignNo);
    }

    [RelayCommand] private Task WithdrawProductAsync() => Task.CompletedTask;
    [RelayCommand] private Task UploadEditImageAsync() => Task.CompletedTask;
    [RelayCommand] private Task OpenRelatedProductsAsync() => Task.CompletedTask;
    [RelayCommand] private Task ChangePriceAsync() => Task.CompletedTask;
    [RelayCommand] private Task AddPromoAsync() => Task.CompletedTask;

    [RelayCommand]
    private async Task LoadLookupsAsync()
    {
        SupplierNameOptions.Clear();
        foreach (var s in await _service.GetSupplierNamesAsync()) SupplierNameOptions.Add(s);

        CountryOptions.Clear();
        foreach (var s in await _service.GetCountryOptionsAsync()) CountryOptions.Add(s);

        DiscontinuedReasonOptions.Clear();
        foreach (var s in await _service.GetDiscontinuedReasonOptionsAsync()) DiscontinuedReasonOptions.Add(s);

        WebOverlayOptions.Clear();
        foreach (var s in await _service.GetWebOverlayOptionsAsync()) WebOverlayOptions.Add(s);

        WebExclusiveOptions.Clear();
        foreach (var s in await _service.GetWebExclusiveOptionsAsync()) WebExclusiveOptions.Add(s);

        WebDeliveryOptions.Clear();
        foreach (var s in await _service.GetWebDeliveryOptionsAsync()) WebDeliveryOptions.Add(s);

        StockStatusOptions.Clear();
        StockStatusOptions.Add("Stocked");
        StockStatusOptions.Add("Direct");
        StockStatusOptions.Add("Back-to-Back");

        WebStoreOptions.Clear();
        WebStoreOptions.Add("1 - UK Store Only");
        WebStoreOptions.Add("2 - ROI Store Only");
        WebStoreOptions.Add("3 - All Stores");
        WebStoreOptions.Add("4 - No Stores");

        YesNoOptions.Clear();
        YesNoOptions.Add("Yes");
        YesNoOptions.Add("No");
    }

    [RelayCommand]
    private Task CheckPriceBreaksAsync()
    {
        NetCost2Invalid = false;
        NetCost3Invalid = false;
        MinQty2Invalid = false;
        MinQty3Invalid = false;

        var n1 = NetCost ?? 0m;
        var n2 = NetCost2 ?? 0m;
        var n3 = NetCost3 ?? 0m;
        var q2 = MinQty2 ?? 0;
        var q3 = MinQty3 ?? 0;

        if (n2 > n1) NetCost2Invalid = true;
        if (n3 > n2) NetCost3Invalid = true;

        if (q2 == 0 && n2 > 0) { MinQty2Invalid = true; NetCost2Invalid = true; }
        if (q2 > 0 && n2 == 0) MinQty2Invalid = true;
        if (q3 == 0 && n3 > 0) MinQty3Invalid = true;
        if (q3 > 0 && n3 == 0) MinQty3Invalid = true;

        HasInvalidPriceBreak = NetCost2Invalid || NetCost3Invalid || MinQty2Invalid || MinQty3Invalid;
        return Task.CompletedTask;
    }

    private void MapFromRow(ProductEditRow r)
    {
        Id = r.Id;
        DesignNo = r.Name;
        PageNo = r.PageNumber;
        ObjectType = r.ObjectType;
        LongDescription1 = r.LongDescription1;
        LongDescription2 = r.LongDescription2;

        SupplierName = _context.SelectedSupplierDetails!.SupplierName;
        SupplierCode = r.SupplierCode;
        SupplierProductCode = r.SupplierProductCode;
        SupplierListPrice = r.SupplierListPrice;
        SupplierDiscount = r.SupplierDiscount;
        NetCost = r.SupplierListPrice * (1 - r.SupplierDiscount);
        NetCost2 = r.NetCostPrice2;
        NetCost3 = r.NetCostPrice3;
        CurrencyFactor = r.CurrencyFactor;
        FreightIn = r.FreightIn;
        FreightOut = r.FreightOut;
        StandardCost = (r.SupplierListPrice * (1 - r.SupplierDiscount)) / r.CurrencyFactor * (r.StockStatus == "Stocked" ? 1 + r.FreightIn : 1);
        CarriageType = r.CarriageType;
        CarriageCharge = r.CarriageCharge;
        ActualLandedCost = r.ActualLandedCost;
        AdditionalCosts = r.AdditionalCosts;

        ZoneA = r.ZoneACarriage;
        ZoneB = r.ZoneBCarriage;
        ZoneC = r.ZoneCCarriage;
        ZoneD = r.ZoneDCarriage;
        ZoneE = r.ZoneECarriage;
        ZoneS = r.ZoneSCarriage;
        ShippingMethod = r.ShippingMethod;
        ShippingCost = r.ShippingCosts;

        TotalCost = (UseActualCost ? r.ActualLandedCost : r.StandardCost) + r.ShippingCosts + (r.AdditionalCosts ?? 0);
        SellingPrice1 = r.Price1;
        SellingPrice2 = r.Price2;
        SellingPrice3 = r.Price3;
        Margin1 = (r.Price1 - r.TotalCost) / r.TotalCost;
        Margin2 = r.Price2 != null ? (r.Price2 - (r.NetCostPrice2 > 0 ? r.NetCostPrice2 : r.TotalCost)) / r.Price2 : null; 
        Margin3 = r.Price3 != null ? (r.Price3 - (r.NetCostPrice3 > 0 ? r.NetCostPrice3 : r.TotalCost)) / r.Price3 : null;
        UkPromoPrice = r.UkPromoPrice;
        RoiPromoPrice = r.RoiPromoPrice;

        ParentId = r.ParentId;
        StockStatus = r.StockStatus;
        GreenFlag = r.GreenFlag;
        CountryOfOrigin = r.CountryOfOrigin;
        LeadTime = r.LeadTime;
        SupplierDueDate = r.SupplierDueDate;
        WebDeliveryIcon = r.WebDeliveryIcon;
        MinOrderQuantity = r.MinOrderQuantity;
        MinQty2 = r.MinQty2;
        MinQty3 = r.MinQty3;
        MadeToOrder = r.MadeToOrder ?? "No";
        MadeInUk = r.ProductMadeInUk ?? (string.Equals(r.CountryOfOrigin, "United Kingdom") ? "Yes" : "No");
        TariffCode = r.TariffCode;
        ManageStock = r.ManageStock;
        Component = r.Component;

        PackagingHeight = r.PackagingHeight;
        PackagingWidth = r.PackagingWidth;
        PackagingLength = r.PackagingLength;
        Weight = r.Weight;
        UsePalletRate = Flag(r.UsePalletRate);
        NoOfPallets = r.NoOfPallets;
        Boxed = Flag(r.Boxed);

        DiscontinuedDate = r.DiscontinuedDate;
        DiscontinuedReason = r.DiscontinuedReason;
        RunToZero = Flag(r.RunToZero);
        Transactional = Flag(r.Transactional);
        ReleaseDate = r.ReleaseDate;
        WebStores = r.WebStores;
        ProductSupplierMessage = r.ProductSupplierMessage;
        ProductMessage = r.ProductAlertMessage;
        MessageStartDate = r.ProductMessageStartDate;
        MessageEndDate = r.ProductMessageEndDate;

        PrimaryImagePath = r.VideoUrl;
        WebOverlay = r.PackFlash;
        WebExclusive = r.WebExclusive;
        ShowRelatedOnWeb = Flag(r.DisplayRelated);
    }

    private ProductEditRow ToRow() => new()
    {
        Id = Id ?? "",
        Name = DesignNo ?? "",
        PageNumber = PageNo,
        ObjectType = ObjectType ?? "",
        LongDescription1 = LongDescription1,
        LongDescription2 = LongDescription2,
        SupplierName = _context.SelectedSupplierDetails!.SupplierName ?? "",
        SupplierCode = SupplierCode,
        SupplierProductCode = SupplierProductCode,
        SupplierListPrice = SupplierListPrice,
        SupplierDiscount = SupplierDiscount,
        StandardCost = StandardCost,
        NetCostPrice2 = NetCost2,
        NetCostPrice3 = NetCost3,
        CurrencyFactor = CurrencyFactor,
        FreightIn = FreightIn,
        FreightOut = FreightOut,
        CarriageType = CarriageType,
        CarriageCharge = CarriageCharge,
        ActualLandedCost = ActualLandedCost,
        AdditionalCosts = AdditionalCosts,
        MinQty2 = MinQty2,
        MinQty3 = MinQty3,
        ZoneACarriage = ZoneA,
        ZoneBCarriage = ZoneB,
        ZoneCCarriage = ZoneC,
        ZoneDCarriage = ZoneD,
        ZoneECarriage = ZoneE,
        ZoneSCarriage = ZoneS,
        ShippingMethod = ShippingMethod,
        ShippingCosts = ShippingCost,
        TotalCost = TotalCost,
        Price1 = SellingPrice1,
        Price2 = SellingPrice2,
        Price3 = SellingPrice3,
        Margin = Margin1,
        UkPromoPrice = UkPromoPrice,
        RoiPromoPrice = RoiPromoPrice,
        ParentId = ParentId ?? "",
        StockStatus = StockStatus,
        GreenFlag = GreenFlag,
        CountryOfOrigin = CountryOfOrigin,
        LeadTime = LeadTime,
        SupplierDueDate = SupplierDueDate,
        WebDeliveryIcon = WebDeliveryIcon,
        MinOrderQuantity = MinOrderQuantity,
        MadeToOrder = MadeToOrder,
        ProductMadeInUk = MadeInUk,
        TariffCode = TariffCode,
        ManageStock = ManageStock,
        Component = Component,
        PackagingHeight = PackagingHeight,
        PackagingWidth = PackagingWidth,
        PackagingLength = PackagingLength,
        Weight = Weight,
        UsePalletRate = Yn(UsePalletRate),
        NoOfPallets = NoOfPallets,
        Boxed = Yn(Boxed),
        DiscontinuedDate = DiscontinuedDate,
        DiscontinuedReason = DiscontinuedReason,
        RunToZero = Yn(RunToZero),
        Transactional = Yn(Transactional),
        ReleaseDate = ReleaseDate,
        WebStores = WebStores,
        ProductSupplierMessage = ProductSupplierMessage,
        ProductAlertMessage = ProductMessage,
        ProductMessageStartDate = MessageStartDate,
        ProductMessageEndDate = MessageEndDate,
        PackFlash = WebOverlay,
        WebExclusive = WebExclusive,
        DisplayRelated = Yn(ShowRelatedOnWeb),
        VideoUrl = PrimaryImagePath
    };

    private void CloseWithResult(bool saved) => RequestClose?.Invoke(this, saved);

    private static bool Flag(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return false;
        var t = v.Trim().ToLowerInvariant();
        return t is "yes" or "y" or "true" or "1";
    }

    private static string Yn(bool b) => b ? "Yes" : "No";

    private static decimal? CalcNetCost(decimal? list, decimal? discount)
    {
        if (!list.HasValue) return null;
        var d = discount ?? 0m;
        return list.Value * (1m - d);
    }

    private static decimal? CalcStandardCost(decimal? netCost, decimal? currencyFactor, decimal? freightIn,
        string? stockStatus)
    {
        if (!netCost.HasValue || !currencyFactor.HasValue || currencyFactor == 0) return null;
        var baseCost = netCost.Value / currencyFactor.Value;
        if (string.Equals(stockStatus, "Stocked", StringComparison.OrdinalIgnoreCase))
            return baseCost * (1m + (freightIn ?? 0m));
        return baseCost;
    }

    private static decimal? CalcTotalCost(bool useActualCost, decimal? actualLanded, decimal? standard,
        decimal? shipping, decimal? addl)
    {
        var baseCost = useActualCost ? (actualLanded ?? 0m) : (standard ?? 0m);
        return baseCost + (shipping ?? 0m) + (addl ?? 0m);
    }

    private static decimal? CalcMargin(decimal? sellPrice, decimal? costBase)
    {
        if (!sellPrice.HasValue || !costBase.HasValue || sellPrice == 0) return null;
        return (sellPrice.Value - costBase.Value) / sellPrice.Value;
    }

    private void TriggerRecompute()
    {
        if (_isInitializing) return;
        var version = Interlocked.Increment(ref _recomputeVersion);
        _ = LoadComputedAsyncGuarded(version);
        MarkDirty();
    }

    public void Dispose()
    {
        _recomputeGate.Dispose();
    }
}