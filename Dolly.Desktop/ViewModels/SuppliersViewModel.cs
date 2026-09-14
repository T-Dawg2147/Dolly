using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;

namespace Dolly.Desktop.ViewModels;

public partial class SuppliersViewModel : ObservableObject
{
    private readonly ISupplierQueriesService _queries;
    private readonly SupplierContextState _context;

    public ObservableCollection<SupplierSummary> Suppliers { get; } = [];

    [ObservableProperty] private SupplierSummary? selectedSupplier;
    [ObservableProperty] private string supplierSearchText = "";

    public SuppliersViewModel(ISupplierQueriesService queries, SupplierContextState context)
    {
        _queries = queries;
        _context = context;
    }

    [RelayCommand]
    private async Task LoadSuppliersAsync()
    {
        var results = await _queries.SearchSuppliersAsync(SupplierSearchText);
        Suppliers.Clear();
        foreach (var s in results) Suppliers.Add(s);
    }

    partial void OnSelectedSupplierChanged(SupplierSummary? value)
    {
        _context.SelectedSupplierCode = value?.SupplierCode;
    }
}