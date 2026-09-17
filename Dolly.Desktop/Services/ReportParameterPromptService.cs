using System.Windows;
using Dolly.Application.Abstraction;

namespace Dolly.Desktop.Services;

public sealed class ReportParameterPromptService : IReportParameterPromptService
{
    public async Task<DateTime?> PromptForDateAsync(string title, string message, CancellationToken ct = default)
    {
        var dialog = new Views.DatePromptWindow(title, message);
        var owner = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
        if (owner is not null) dialog.Owner = owner;

        var result = dialog.ShowDialog();
        return result == true ? dialog.SelectedDate : null;
    }

    public Task<string?> PromptForHierarchyLeafAsync(CancellationToken ct = default)
    {
        var dialog = new Views.HierarchySelectorWindow();
        var result = dialog.ShowDialog();
        return Task.FromResult(result == true ? dialog.SelectedLeafId : null);
    }
}