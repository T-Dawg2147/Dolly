namespace Dolly.Application.Abstraction;

public interface IUnspscService
{
    Task ApplyUnspscAsync(
        string groupId,
        string segmentTitle,
        string commodityTitle,
        string commodityCode,
        bool batch,
        CancellationToken ct = default);
}