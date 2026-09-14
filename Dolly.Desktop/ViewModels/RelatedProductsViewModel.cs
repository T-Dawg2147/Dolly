using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;
using Dolly.Desktop.Services;

namespace Dolly.Desktop.ViewModels;

/// <summary>
/// Unified replacement for frm_Related + frm_RelatedProducts.
/// </summary>
public partial class RelatedProductsViewModel(
    IRelatedProductsService service,
    IAddRelatedProductDialogService addDialog) : ObservableObject
{
    private readonly IRelatedProductsService _service = service;
    private readonly IAddRelatedProductDialogService _addDialog = addDialog;

    [ObservableProperty] private string productCode = "";
    [ObservableProperty] private string username = "";
    [ObservableProperty] private RelatedProductRow? selectedRelated;

    [ObservableProperty] private bool hasPendingChanges;
    [ObservableProperty] private string closeButtonText = "Close";

    public ObservableCollection<RelatedProductRow> Related { get; } = [];
    public event EventHandler? RequestClose;

    [RelayCommand]
    public async Task LoadAsync(string productCode)
    {
        ProductCode = productCode;
        HasPendingChanges = false;
        CloseButtonText = "Close";
        await RefreshAsync();
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        Related.Clear();
        var rows = await _service.GetRelatedAsync(ProductCode);
        foreach (var r in rows) Related.Add(r);
    }

    [RelayCommand]
    public async Task OpenAddDialogAsync()
    {
        var added = await _addDialog.ShowAsync(ProductCode);
        if (!added) return;

        MarkDirty();
        await RefreshAsync();
    }

    [RelayCommand]
    public async Task UnlinkSelectedAsync()
    {
        if (SelectedRelated?.RelatedId is null) return;

        var response = MessageBox.Show(
            $"This will remove the related product code {SelectedRelated.RelatedId} from {ProductCode}. Do you wish to continue?",
            "Unlink product",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information);

        if (response != MessageBoxResult.Yes) return;

        await _service.UnlinkByRefAsync(ProductCode, SelectedRelated.RelatedId);
        MarkDirty();
        await RefreshAsync();
    }

    [RelayCommand]
    public async Task CloseOrSaveAsync()
    {
        if (HasPendingChanges)
        {
            await _service.CommitRelatedChangesAsync(ProductCode, Username);
            MessageBox.Show("Update complete", "Related Products", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    public void Cancel()
    {
        // parity: close without save
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private void MarkDirty()
    {
        HasPendingChanges = true;
        CloseButtonText = "Save";
    }
}