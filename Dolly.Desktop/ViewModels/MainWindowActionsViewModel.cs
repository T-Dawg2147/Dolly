using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dolly.Application.Abstraction;

namespace Dolly.Desktop.ViewModels;

/// <summary>
/// MIGRATION NOTE:
/// Consolidates legacy button-click VBA from main window into command-based VM.
/// </summary>
public sealed partial class MainWindowActionsViewModel : ObservableObject
{
    private readonly IReportExportService _reports;
    private readonly IPublicationService _publications;
    private readonly IUnspscService _unspsc;
    private readonly ISearchService _search;
    private readonly IFolderPickerService _folderPicker;
    private readonly ISearchResultsDialogService _searchDialog;
    
    private readonly SupplierContextState _context;

    [ObservableProperty] private string? _selectedSupplierCode;
    [ObservableProperty] private bool _reportingSupplier;
    [ObservableProperty] private bool _batchMode;
    [ObservableProperty] private bool _suppressWarnings;

    [ObservableProperty] private string _publicationName = "";
    [ObservableProperty] private DateTime? _publicationStartDate;
    [ObservableProperty] private DateTime? _publicationEndDate;
    [ObservableProperty] private string _mediaCodeUk = "";
    [ObservableProperty] private string _mediaCodeRoi = "";
    [ObservableProperty] private string _priceListType = "";
    [ObservableProperty] private bool _publicationIsUpdate;

    [ObservableProperty] private string _unspscGroupId = "";
    [ObservableProperty] private string _unspscSegmentTitle = "";
    [ObservableProperty] private string _unspscCommodityTitle = "";
    [ObservableProperty] private string _unspscCommodityCode = "";

    [ObservableProperty] private string _searchTerm = "";

    public MainWindowActionsViewModel(
        IReportExportService reports,
        IPublicationService publications,
        IUnspscService unspsc,
        ISearchService search,
        IFolderPickerService folderPicker,
        ISearchResultsDialogService searchDialog,
        SupplierContextState context)
    {
        _reports = reports;
        _publications = publications;
        _unspsc = unspsc;
        _search = search;
        _folderPicker = folderPicker;
        _searchDialog = searchDialog;
        _context = context;
    }

    [RelayCommand]
    private async Task UpdateCreatePublicationAsync()
    {
        ValidatePublication();
        await _publications.CreateOrUpdatePublicationAsync(
            PublicationName,
            PublicationStartDate!.Value,
            PublicationEndDate!.Value,
            MediaCodeUk,
            MediaCodeRoi,
            PriceListType,
            PublicationIsUpdate);
    }

    [RelayCommand]
    private async Task UseUnspscAsync()
    {
        if (string.IsNullOrWhiteSpace(UnspscCommodityCode))
            throw new InvalidOperationException("No UNSPSC commodity selected.");

        await _unspsc.ApplyUnspscAsync(
            UnspscGroupId,
            UnspscSegmentTitle,
            UnspscCommodityTitle,
            UnspscCommodityCode,
            BatchMode);
    }

    /*[RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm)) return;
        var results = await _search.SearchAsync(SearchTerm);
        if (results.Count == 0)
            throw new InvalidOperationException($"No results for '{SearchTerm}'.");
        
        // TODO - Open SearchResults dialog and bind results collection
    }
    */

    [RelayCommand]
    private async Task OpenSearchPopupAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm)) return;

        var nav = await _searchDialog.ShowAsync(SearchTerm);
        if (nav is null) return;

        _context.SelectedSupplierCode = nav.SupplierCode;
        _context.SelectedProductCode = nav.ProductCodeOrName;
        _context.GroupFilterParentId = nav.GroupFilterParentId;

        // TODO: If GroupFilterParentId is set, apply product grid filter by Parent_ID in ProductsViewModel.
    }

    [RelayCommand]
    private async Task ExportRunToZeroAsync(string folder) =>
        await _reports.ExportRunToZeroAsync(folder);

    [RelayCommand]
    private async Task ExportSupplierReportAsync(string folder)
    {
        if (string.IsNullOrWhiteSpace(SelectedSupplierCode))
            throw new InvalidOperationException("No supplier selected.");
        await _reports.ExportSupplierReportAsync(folder, SelectedSupplierCode, ReportingSupplier);
    }
    
    [RelayCommand]
    private async Task ExportRunToZeroWithPickerAsync()
    {
        var folder = await _folderPicker.PickFolderAsync("Select Folder to Export");
        if (string.IsNullOrWhiteSpace(folder)) return;
        await _reports.ExportRunToZeroAsync(folder);
    }

    [RelayCommand]
    private async Task ExportSupplierReportWithPickerAsync()
    {
        var folder = await _folderPicker.PickFolderAsync("Select Folder to Export");
        if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(SelectedSupplierCode)) return;
        await _reports.ExportSupplierReportAsync(folder, SelectedSupplierCode, ReportingSupplier);
    }

    private void ValidatePublication()
    {
        if (string.IsNullOrWhiteSpace(PublicationName))
            throw new InvalidOperationException("Publication name is required.");
        if (!PublicationStartDate.HasValue || !PublicationEndDate.HasValue)
            throw new InvalidOperationException("Start and End dates are required.");
        if (PublicationEndDate <= PublicationStartDate)
            throw new InvalidOperationException("End date must be after Start date.");
        if (string.IsNullOrWhiteSpace(MediaCodeUk))
            throw new InvalidOperationException("UK media code is required.");
        if (string.IsNullOrWhiteSpace(PriceListType))
            throw new InvalidOperationException("Price list type is required.");
    }
}