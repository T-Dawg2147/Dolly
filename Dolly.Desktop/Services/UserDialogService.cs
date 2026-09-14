using System.Windows;
using Dolly.Application.Abstraction;

namespace Dolly.Desktop.Services;

public sealed class UserDialogService : IUserDialogService
{
    public async Task ShowInfoAsync(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public async Task ShowErrorAsync(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public async Task<bool> ConfirmYesNoAsync(string message, string title)
    {
        var r = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return r == MessageBoxResult.Yes;
    }
}