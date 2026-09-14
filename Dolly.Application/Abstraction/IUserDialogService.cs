namespace Dolly.Application.Abstraction;

public interface IUserDialogService
{
    Task ShowInfoAsync(string message, string title);
    Task ShowErrorAsync(string message, string title);
    Task<bool> ConfirmYesNoAsync(string message, string title);
}