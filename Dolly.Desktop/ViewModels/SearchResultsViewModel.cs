using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;

namespace Dolly.Desktop.ViewModels;

public partial class SearchResultsViewModel(ISearchService search, SupplierContextState context) : ObservableObject
{
    private readonly ISearchService _search = search;
    private readonly SupplierContextState _context = context;

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private bool _showExactSection;
    [ObservableProperty] private SearchResultRow? _selectedResult;
    [ObservableProperty] private string _foundSummary = "Found 0 occurrences";
    [ObservableProperty] private string _exactSummaryText = "";

    public ObservableCollection<SearchResultRow> Results { get; } = [];
    public ObservableCollection<SearchResultRow> ExactResults { get; } = [];

    public event EventHandler<SearchNavigationTarget?> NavigateRequested;
    public event EventHandler? CloseRequested;

    [RelayCommand]
    public async Task LoadAsync(string term)
    {
        SearchText = term;
        await RunSearchAsync();
    }

    [RelayCommand]
    public async Task RunSearchAsync()
    {
        var rows = await _search.SearchAsync(SearchText);
        Results.Clear();
        ExactResults.Clear();

        foreach (var r in rows)
        {
            Results.Add(r);
            if (!string.IsNullOrWhiteSpace(r.ExactFound))
                ExactResults.Add(r);
        }

        ShowExactSection = ExactResults.Count > 0;
        FoundSummary = $"Found {Results.Count} occurrences";
        ExactSummaryText = ShowExactSection
            ? $"{ExactResults.Count} exact match(es) found"
            : "No exact match";
    }
    
    [RelayCommand]
    public async Task OpenSelectedAsync()
    {
        if (SelectedResult is null) return;
        await OpenRowAsync(SelectedResult);
    }

    [RelayCommand]
    public async Task OpenFirstExactAsync()
    {
        if (ExactResults.Count == 0) return;
        await OpenRowAsync(ExactResults[0]);
    }

    public async Task<bool> TryQuickOpenSingleExactAsync()
    {
        if (ExactResults.Count != 1) return false;
        await OpenRowAsync(ExactResults[0]);
        return true;
    }

    private async Task OpenRowAsync(SearchResultRow row)
    {
        var target = await _search.ResolveNavigationTargetAsync(row, _context.ReportingSupplier);
        if (target is null) return;

        NavigateRequested?.Invoke(this, target);
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}