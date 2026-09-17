using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;

namespace Dolly.Desktop.ViewModels.Layout;

public sealed partial class ProductsPaneViewModel : ObservableObject
{
    private readonly ISupplierQueriesService _queries;
    private readonly SupplierContextState _context;
    private readonly IProductEditDialogService _editDialog;

    private bool _suppressSelectionSync;

    public ObservableCollection<SupplierProductRow> Products { get; } = [];
    public ObservableCollection<SupplierProductSalesRow> Sales { get; } = [];
    public ICollectionView ProductsView { get; }

    [ObservableProperty] private SupplierProductRow? _selectedProduct;
    [ObservableProperty] private SupplierProductSalesRow? _selectedSalesRow;
    [ObservableProperty] private string? _activeParentFilter;
    [ObservableProperty] private bool _isGroupFilterActive;

    public ProductsPaneViewModel(ISupplierQueriesService queries, SupplierContextState context, IProductEditDialogService editDialog)
    {
        _queries = queries;
        _context = context;
        _editDialog = editDialog;

        ProductsView = CollectionViewSource.GetDefaultView(Products);
        ProductsView.Filter = FilterProduct;

        _context.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName is nameof(SupplierContextState.SelectedSupplierCode)
                or nameof(SupplierContextState.ProductStatus))
            {
                await ReloadProductsAsync();
                await ReloadSalesAsync();
            }

            if (e.PropertyName == nameof(SupplierContextState.GroupFilterParentId))
                ApplyGroupFilter(_context.GroupFilterParentId);

            if (e.PropertyName == nameof(SupplierContextState.SelectedProductCode))
                TrySelectByProductCode(_context.SelectedProductCode);
        };
    }

    [RelayCommand]
    public async Task ReloadProductsAsync()
    {
        Products.Clear();

        if (string.IsNullOrWhiteSpace(_context.SelectedSupplierCode))
            return;

        var rows = await _queries.GetSupplierProductsAsync(_context.SelectedSupplierCode!, _context.ProductStatus);
        foreach (var row in rows) Products.Add(row);
        
        ApplyGroupFilter(_context.GroupFilterParentId);

        if (!string.IsNullOrWhiteSpace(_context.SelectedProductCode))
            TrySelectByProductCode(_context.SelectedProductCode);
        else if (Products.Count > 0)
            SelectedProduct = Products[0];
    }

    [RelayCommand]
    public async Task ReloadSalesAsync()
    {
        Sales.Clear();

        if (string.IsNullOrWhiteSpace(_context.SelectedSupplierCode))
            return;

        var rows = await _queries.GetSupplierProductSalesAsync(_context.SelectedSupplierCode!, _context.ProductStatus);
        foreach (var row in rows) Sales.Add(row);

        if (!string.IsNullOrWhiteSpace(_context.SelectedProductCode))
            TrySelectByProductCode(_context.SelectedProductCode);
    }

    [RelayCommand]
    private void ClearGroupFilter()
    {
        _context.GroupFilterParentId = null;
        ApplyGroupFilter(null);
    }

    [RelayCommand]
    private async Task OpenSelectedProductEditAsync()
    {
        if (SelectedProduct is null) return;

        if (string.IsNullOrWhiteSpace(_context.SelectedProductCode)) return;

        var result = await _editDialog.ShowAsync(_context.SelectedProductCode);
        if (result == true)
        {
            await ReloadProductsAsync();
            await ReloadSalesAsync();
            if (!string.IsNullOrWhiteSpace(_context.SelectedProductCode))
                _context.SelectedProductOverview = await _queries.GetProductOverviewAsync(_context.SelectedProductCode);
        }
    }

    partial void OnSelectedProductChanged(SupplierProductRow? value)
    {
        if (_suppressSelectionSync || value is null) return;
        SyncSelectionFromProducts(value);
    }

    partial void OnSelectedSalesRowChanged(SupplierProductSalesRow? value)
    {
        if (_suppressSelectionSync || value is null) return;
        SyncSelectionFromSales(value);
    }

    private void SyncSelectionFromProducts(SupplierProductRow value)
    {
        _suppressSelectionSync = true;
        try
        {
            var code = value.DesignNo;
            _context.SelectedProductCode = code;

            if (!string.IsNullOrWhiteSpace(code))
            {
                var match = Sales.FirstOrDefault(s =>
                    string.Equals(s.DesignNo, code, StringComparison.OrdinalIgnoreCase));
                if (match is not null)
                    SelectedSalesRow = match;
            }
        }
        finally
        {
            _suppressSelectionSync = false;
        }
    }
    
    private void SyncSelectionFromSales(SupplierProductSalesRow value)
    {
        _suppressSelectionSync = true;
        try
        {
            var code = value.DesignNo;
            _context.SelectedProductCode = code;

            if (!string.IsNullOrWhiteSpace(code))
            {
                var match = Products.FirstOrDefault(p =>
                    string.Equals(p.DesignNo, code, StringComparison.OrdinalIgnoreCase));
                if (match is not null)
                    SelectedProduct = match;
            }
        }
        finally
        {
            _suppressSelectionSync = false;
        }
    }
    
    private void TrySelectByProductCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code)) return;

        _suppressSelectionSync = true;
        try
        {
            var p = Products.FirstOrDefault(x => string.Equals(x.DesignNo?.ToString(), code, StringComparison.OrdinalIgnoreCase));
            if (p is not null) SelectedProduct = p;

            var s = Sales.FirstOrDefault(x => string.Equals(x.DesignNo, code, StringComparison.OrdinalIgnoreCase));
            if (s is not null) SelectedSalesRow = s;
        }
        finally
        {
            _suppressSelectionSync = false;
        }
    }
    
    private void ApplyGroupFilter(string? parentId)
    {
        ActiveParentFilter = string.IsNullOrWhiteSpace(parentId) ? null : parentId;
        IsGroupFilterActive = !string.IsNullOrWhiteSpace(ActiveParentFilter);
        ProductsView.Refresh();
    }

    private bool FilterProduct(object obj)
    {
        if (obj is not SupplierProductRow p) return false;
        if (string.IsNullOrWhiteSpace(ActiveParentFilter)) return true;
        return string.Equals(p.Parent, ActiveParentFilter, StringComparison.OrdinalIgnoreCase);
    }
}