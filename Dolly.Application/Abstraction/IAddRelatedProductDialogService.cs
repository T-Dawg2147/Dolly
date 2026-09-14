namespace Dolly.Application.Abstraction;

public interface IAddRelatedProductDialogService
{
    Task<bool> ShowAsync(string productCode, CancellationToken ct = default);
}