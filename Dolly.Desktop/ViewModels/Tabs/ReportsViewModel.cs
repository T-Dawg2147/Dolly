using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;

namespace Dolly.Desktop.ViewModels.Tabs;

public sealed partial class ReportsViewModel : ObservableObject
{
    private readonly IReportCatalogService _catalog;
    private readonly IGenericReportRunner _runner;
    private readonly IReportParameterPromptService _paramPrompt;
    private readonly IFolderPickerService _folderPicker;
    private readonly SupplierContextState _context;

    public ObservableCollection<ReportDefinition> AllReports { get; } = [];
    public ICollectionView FilteredReports { get; }

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string? _statusMessage;

    public ReportsViewModel(
        IReportCatalogService catalog,
        IGenericReportRunner runner,
        IReportParameterPromptService paramPrompt,
        IFolderPickerService folderPicker,
        SupplierContextState context)
    {
        _catalog = catalog;
        _runner = runner;
        _paramPrompt = paramPrompt;
        _folderPicker = folderPicker;
        _context = context;

        FilteredReports = CollectionViewSource.GetDefaultView(AllReports);
        FilteredReports.Filter = FilterReport;
        FilteredReports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ReportDefinition.Category)));
        // FIXED: was `nameof(ReportDefinition)` — see Bug 5 above

        _ = LoadReportsAsync();
    }

    [RelayCommand] // CHANGED: was a private plain method with no way to manually refresh from the UI
    private async Task LoadReportsAsync()
    {
        var reports = await _catalog.GetReportsAsync();
        AllReports.Clear();
        foreach (var r in reports) AllReports.Add(r);
    }

    partial void OnSearchTextChanged(string value) => FilteredReports.Refresh();

    private bool FilterReport(object obj)
    {
        if (obj is not ReportDefinition report) return false;
        if (string.IsNullOrWhiteSpace(SearchText)) return true;

        return report.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
               || (report.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
               || report.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
    }

    // ADDED: standalone command for the folder button, completely separate from the report catalog.
    [RelayCommand]
    private async Task OpenSpecialsFolderAsync()
    {
        var folder = await _folderPicker.PickFolderAsync("Open Specials Report Folder");
        if (folder is not null)
            System.Diagnostics.Process.Start("explorer.exe", folder);
    }

    [RelayCommand]
    private async Task RunReportAsync(ReportDefinition? report)
    {
        if (report is null) return;

        // REMOVED: the `if (report.IconKind == "Folder")` branch that lived here.
        // "Open Specials Report Folder" is no longer part of the report catalog at all —
        // see OpenSpecialsFolderAsync above.

        var extraParameters = new Dictionary<string, object?>();

        switch (report.ParameterMode)
        {
            case "Date":
                var chosenDate = await _paramPrompt.PromptForDateAsync(
                    report.Title, "Please choose the month/year for this report.");
                if (chosenDate is null)
                {
                    StatusMessage = "Export has been cancelled.";
                    return;
                }
                extraParameters["ForDate"] = chosenDate.Value;
                break;

            case "HierarchyLeaf":
                var leafId = await _paramPrompt.PromptForHierarchyLeafAsync();
                if (string.IsNullOrWhiteSpace(leafId))
                {
                    StatusMessage = "Report cancelled - no hierarchy node selected.";
                    return;
                }
                extraParameters["LeafId"] = leafId;
                break;
        }

        var outputFolder = await _folderPicker.PickFolderAsync($"Save {report.Title}");
        if (outputFolder is null)
        {
            StatusMessage = "Export has been cancelled.";
            return;
        }

        IsBusy = true;
        StatusMessage = $"Running '{report.Title}'...";
        try
        {
            await _runner.RunAsync(
                report, outputFolder, _context.SelectedSupplierCode, _context.ReportingSupplier, extraParameters);

            StatusMessage = $"Report has been exported to: {outputFolder}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error running '{report.Title}': {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}