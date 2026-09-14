using Dolly.Application.Models;

namespace Dolly.Application.Abstraction;

public interface IChangeCommitService
{
    Task<ApplyChangesResult> ApplyChangesAsync(
        string supplierCode,
        string productCode,
        string objectType,
        string username,
        CancellationToken ct = default);
}