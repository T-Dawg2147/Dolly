namespace Dolly.Application.Abstraction;

public interface IFolderPickerService
{
    Task<string?> PickFolderAsync(string title, CancellationToken ct = default);
}