using Dolly.Application.Abstraction;

namespace Dolly.Desktop.Services;

public class CurrentUserService : ICurrentUserService
{
    public static string Username => Environment.UserName;
}