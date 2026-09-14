using Dolly.Application.Models;

namespace Dolly.Application.Abstraction;

public interface IRelatedProductsService
{
    Task<IReadOnlyList<RelatedProductRow>> GetRelatedAsync(string productCode, CancellationToken ct = default);
    Task<RelatedProductPreviewRow?> GetRelatedPreviewAsync(string relatedCode, CancellationToken ct = default);

    Task<AddRelatedResult> AddRelatedWithStateAsync(
        string relatedParentCode,
        string relatedCode,
        string username,
        CancellationToken ct = default);

    Task UnlinkByRefAsync(string productCode, string relatedCode, CancellationToken ct = default);
    Task CommitRelatedChangesAsync(string productCode, string username, CancellationToken ct = default);
}