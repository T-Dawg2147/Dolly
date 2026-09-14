using Dolly.Application.Abstraction;
using Microsoft.Win32;

namespace Dolly.Desktop.Services;

/// <summary>
/// WPF/Windows API folder picker. I REFUSE to use WinForms.
/// Requires .NET with OpenFileDialog available (Win11 SDK / modern Windows Desktop).
/// </summary>
public sealed class FolderPickerService : IFolderPickerService
{
    public Task<string?> PickFolderAsync(string title, CancellationToken ct = default)
    {
        var dlg = new OpenFolderDialog
        {
            Title = title,
            Multiselect = false
        };

        var ok = dlg.ShowDialog() == true;
        return Task.FromResult(ok ? dlg.FolderName : null);
    }
}