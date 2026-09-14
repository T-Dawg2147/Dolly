namespace Dolly.Application.Abstraction;

public interface IProductImageResolver
{
    Task<string?> ResolveImagePathAsync(string codeOrName, CancellationToken ct = default);
}