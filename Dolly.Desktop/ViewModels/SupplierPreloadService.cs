using Dolly.Application.Abstraction;

namespace Dolly.Desktop.ViewModels;

public sealed class SupplierPreloadService
{
    private readonly ISupplierQueriesService _queries;
    private readonly ISupplierDetailsCache _cache;

    public SupplierPreloadService(ISupplierQueriesService queries, ISupplierDetailsCache cache)
    {
        _queries = queries;
        _cache = cache;
    }

    public async Task WarmAsync(IEnumerable<string> supplierCodes, CancellationToken ct = default)
    {
        foreach (var code in supplierCodes.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (_cache.TryGet(code, out _)) continue;
            var row = await _queries.GetSupplierDetailsAsync(code, ct);
            if (row is not null) _cache.Set(code, row);
        }
    }
}