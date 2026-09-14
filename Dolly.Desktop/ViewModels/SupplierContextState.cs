using CommunityToolkit.Mvvm.ComponentModel;
using Dolly.Application.Models;

namespace Dolly.Desktop.ViewModels;

public partial class SupplierContextState : ObservableObject
{
    [ObservableProperty] private string? _selectedSupplierCode;
    [ObservableProperty] private string? _selectedProductCode;
    [ObservableProperty] private int _productStatus = 1;

    [ObservableProperty] private SupplierDetailsRow? _selectedSupplierDetails;
    [ObservableProperty] private ProductOverviewRow? _selectedProductOverview;
    
    [ObservableProperty] private bool _reportingSupplier;
    [ObservableProperty] private bool _containsSupplier;

    [ObservableProperty] private string? _groupFilterParentId;
}