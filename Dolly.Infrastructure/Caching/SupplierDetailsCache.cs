using Dolly.Application.Abstraction;
using Dolly.Application.Models;

namespace Dolly.Infrastructure.Caching;

public sealed class SupplierDetailsCache : ISupplierDetailsCache
{
    private readonly Dictionary<string, SupplierDetailsRow> _cache = new(StringComparer.OrdinalIgnoreCase);

    public bool TryGet(string supplierCode, out SupplierDetailsRow row) => _cache.TryGetValue(supplierCode, out row!);
    public void Set(string supplierCode, SupplierDetailsRow row) => _cache[supplierCode] = row;
    public void Clear() => _cache.Clear();
    public void Remove(string supplierCode) => _cache.Remove(supplierCode);
}