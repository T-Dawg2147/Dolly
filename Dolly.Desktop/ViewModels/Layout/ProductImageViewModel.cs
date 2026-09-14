using CommunityToolkit.Mvvm.ComponentModel;
using Dolly.Application.Abstraction;

namespace Dolly.Desktop.ViewModels.Layout;

public partial class ProductImageViewModel : ObservableObject
{
    private readonly IProductEditService _queries;
    private readonly SupplierContextState _context;
    
    [ObservableProperty] private string? _currentImagePath;

    public ProductImageViewModel(IProductEditService queries, SupplierContextState context)
    {
        _queries = queries;
        _context = context;

        _context.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(SupplierContextState.SelectedProductCode))
                await LoadImageAsync(_context.SelectedProductCode);
        };
    }

    private async Task LoadImageAsync(string? productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            CurrentImagePath = null;
            return;
        }

        CurrentImagePath = await _queries.GetProductImagePathAsync(productCode);
    }
}