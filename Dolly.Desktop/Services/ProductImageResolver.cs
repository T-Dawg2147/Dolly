using Dolly.Application.Abstraction;

namespace Dolly.Desktop.Services;

public sealed class ProductImageResolver(IProductEditService queries) : IProductImageResolver
{
    public async Task<string?> ResolveImagePathAsync(string codeOrName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(codeOrName)) return null;

        var stepPath = await queries.GetProductImagePathAsync(codeOrName, ct);
        if (!string.IsNullOrWhiteSpace(stepPath))
            return stepPath;

        return $"https://slingsby.s3-eu-west-1.amazonaws.com/pub/media/import/{{codeOrName.ToLowerInvariant()}}.jpg";
    }
}