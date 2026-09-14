using Dolly.Application.Models;

namespace Dolly.Application.Abstraction;

public interface ISupplierDetailsCache
{
    bool TryGet(string supplierCode, out SupplierDetailsRow row);
    void Set(string supplierCode, SupplierDetailsRow row);
    void Clear();
    void Remove(string supplierCode);
}