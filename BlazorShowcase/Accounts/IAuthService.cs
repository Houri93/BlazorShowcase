using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorShowcase.Accounts;
public interface IAuthService
{
    Session Session { get; }
    User User { get; }

    Task CreateDefaultsAsync();
    Task LogoutAsync();
    Task<bool> TryLoginAsync(string username, string hash);
}