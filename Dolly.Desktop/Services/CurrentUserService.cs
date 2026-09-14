using Dolly.Application.Abstraction;

namespace Dolly.Desktop.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    public string Username => Environment.UserName;
}