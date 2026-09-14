namespace Dolly.Application.Abstraction;

public interface IPublicationService
{
    Task CreateOrUpdatePublicationAsync(
        string publicationName,
        DateTime startDate,
        DateTime endDate,
        string mediaCodeUk,
        string mediaCodeRoi,
        string priceListType,
        bool isUpdate,
        CancellationToken ct = default);
}