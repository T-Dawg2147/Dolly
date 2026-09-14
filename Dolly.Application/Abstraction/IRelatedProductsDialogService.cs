namespace Dolly.Desktop.Services;

public interface IRelatedProductsDialogService
{
    Task ShowAsync(string productCode, string username, CancellationToken ct = default);
}