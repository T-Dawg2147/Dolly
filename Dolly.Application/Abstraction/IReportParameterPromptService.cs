namespace Dolly.Application.Abstraction;

public interface IReportParameterPromptService
{
    Task<DateTime?> PromptForDateAsync(string title, string message, CancellationToken ct = default);
    Task<string?> PromptForHierarchyLeafAsync(CancellationToken ct = default);
}