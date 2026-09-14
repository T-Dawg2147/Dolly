using System.Windows;
using System.Windows.Input;
using DocumentFormat.OpenXml.Drawing.Charts;
using Dolly.Desktop.ViewModels;

namespace Dolly.Desktop.Views;

public partial class SearchResultsWindow : Window
{
    public SearchResultsWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SearchResultsViewModel vm) return;
        vm.CloseRequested += (_, _) => Close();
        SearchTextBox.Focus();
        SearchTextBox.SelectAll();
    }

    private async void ResultsGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is SearchResultsViewModel vm && vm.OpenSelectedCommand.CanExecute(null))
            await vm.OpenSelectedCommand.ExecuteAsync(null);
    }

    private async void SearchTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not SearchResultsViewModel vm) return;

        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            await vm.RunSearchCommand.ExecuteAsync(null);

            var opened = await vm.TryQuickOpenSingleExactAsync();
            if (!opened && vm.Results.Count > 0 && vm.SelectedResult is null)
                vm.SelectedResult = vm.Results[0];
        }
    }
}