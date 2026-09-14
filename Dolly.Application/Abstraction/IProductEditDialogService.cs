namespace Dolly.Application.Abstraction;

public interface IProductEditDialogService
{
    Task<bool?> ShowAsync(string productId, CancellationToken ct = default);
}