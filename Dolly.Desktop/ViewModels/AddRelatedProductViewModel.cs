using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dolly.Application.Abstraction;
using Dolly.Application.Models;

namespace Dolly.Desktop.ViewModels;

public partial class AddRelatedProductViewModel(
    IRelatedProductsService relatedService,
    ICurrentUserService currentUser) : ObservableObject
{
    private readonly IRelatedProductsService _relatedService = relatedService;
    private readonly ICurrentUserService _currentUser = currentUser;

    [ObservableProperty] private string relatedParent = "";
    [ObservableProperty] private string relatedCode = "";
    [ObservableProperty] private RelatedProductPreviewRow? preview;

    public event EventHandler? RequestClose;
    public event EventHandler? AddedSuccessfully;

    [RelayCommand]
    public Task LoadAsync(string parentCode)
    {
        RelatedParent = parentCode;
        RelatedCode = "";
        Preview = null;
        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task LoadPreviewAsync()
    {
        if (string.IsNullOrWhiteSpace(RelatedCode))
        {
            Preview = null;
            return;
        }

        Preview = await _relatedService.GetRelatedPreviewAsync(RelatedCode);
    }

    [RelayCommand]
    public async Task AddAsync()
    {
        if (string.IsNullOrWhiteSpace(RelatedParent) || string.IsNullOrWhiteSpace(RelatedCode)) return;

        var result = await _relatedService.AddRelatedWithStateAsync(RelatedParent, RelatedCode, _currentUser.Username);

        if (string.Equals(result.State, "Exists", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                $"The code {RelatedCode} is already set up as a related product for {RelatedParent}.",
                "Reference already exists",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(
                $"Code {RelatedCode} has been successfully added as a related product to {RelatedParent}.",
                "Reference added",
                MessageBoxButton.OK, MessageBoxImage.Information);

            AddedSuccessfully?.Invoke(this, EventArgs.Empty);
        }

        // parity with Related_ResetForm
        RelatedCode = "";
        Preview = null;
    }

    [RelayCommand]
    private void Close() => RequestClose?.Invoke(this, EventArgs.Empty);
}