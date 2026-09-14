using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;

namespace Dolly.Desktop.ViewModels.Layout;

public partial class ProductOverviewPanelViewModel : ObservableObject
{
    private readonly ISupplierQueriesService _queries;
    private readonly SupplierContextState _context;
    
    [ObservableProperty] private string? _headerText;
    [ObservableProperty] private string? _partNo;
    [ObservableProperty] private string? _moq;
    [ObservableProperty] private string? _stockStatus;
    [ObservableProperty] private string? _leadTime;
    [ObservableProperty] private string? _stockControl;
    [ObservableProperty] private string? _dateIntoStock;
    [ObservableProperty] private string? _nextDay;
    [ObservableProperty] private string? _availableStock;
    [ObservableProperty] private string? _runToZero;
    [ObservableProperty] private string? _backOrders;
    [ObservableProperty] private string? _listPrice;
    [ObservableProperty] private string? _discount;
    [ObservableProperty] private string? _netCost;
    [ObservableProperty] private string? _landedCost;
    [ObservableProperty] private string? _carriage;
    [ObservableProperty] private string? _additionalCosts;
    [ObservableProperty] private string? _totalCost;
    [ObservableProperty] private string? _sellingPriceEuro;
    [ObservableProperty] private string? _salesMessage;

    public ProductOverviewPanelViewModel(ISupplierQueriesService queries, SupplierContextState context)
    {
        _queries = queries;
        _context = context;

        _context.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(SupplierContextState.SelectedProductCode))
                await LoadOverviewAsync(_context.SelectedProductCode);

            if (e.PropertyName == nameof(SupplierContextState.SelectedProductOverview))
                Map(_context.SelectedProductOverview);
        };
    }

    private async Task LoadOverviewAsync(string? productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            _context.SelectedProductOverview = null;
            Clear();
            return;
        }

        var row = await _queries.GetProductOverviewAsync(productCode);
        _context.SelectedProductOverview = row; // shared context, source o' truth
        Map(row);
    }

    private void Map(ProductOverviewRow? row)
    {
        if (row is null)
        {
            Clear();
            return;
        }

        HeaderText = row.Description;
        PartNo = row.PartNo;
        Moq = row.Moq.ToString();
        StockStatus = row.StockStatus;
        LeadTime = row.LeadTime.ToString();
        StockControl = row.StockControl;
        DateIntoStock = row.DateIntoStock;
        NextDay = row.NextDay;
        AvailableStock = row.AvailableStock.ToString();
        RunToZero = row.RunToZero;
        BackOrders = row.BackOrders.ToString();
        ListPrice = row.ListPrice.ToString(CultureInfo.CurrentCulture);
        Discount = row.Discount;
        NetCost = row.NetCost.ToString(CultureInfo.CurrentCulture);
        LandedCost = row.LandedCost;
        Carriage = row.Carriage;
        AdditionalCosts = row.AdditionalCosts.ToString(CultureInfo.CurrentCulture);
        TotalCost = row.TotalCost.ToString(CultureInfo.CurrentCulture);
        SellingPriceEuro = row.SellingPriceEuro.ToString(CultureInfo.CurrentCulture);
        SalesMessage = row.SalesMessage;
    }

    private void Clear()
    {
        HeaderText = PartNo = Moq = StockStatus = LeadTime = StockControl = DateIntoStock =
            NextDay = AvailableStock = RunToZero = BackOrders = ListPrice = Discount = NetCost =
                LandedCost = Carriage = AdditionalCosts = TotalCost = SellingPriceEuro = SalesMessage = null;
    }
}