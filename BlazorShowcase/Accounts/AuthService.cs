using Blazored.LocalStorage;

using BlazorShowcase.DataAccess;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using MudBlazor;

using System.Collections.Concurrent;
using System.Security.Claims;

namespace BlazorShowcase.Accounts;

public class AuthService : AuthenticationStateProvider, IAuthService
{
    private const string AuthType = "apiauth_type";
    private const string TokenKeyName = "MultiLab_Auth";
    private const string ProtectionPurpose = TokenKeyName + AuthType;

    private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<int, AuthService>> globalServerAuths = new();
    private static volatile int idGen = 0;

    public static async Task LogoutAllSessionsAsync(User user)
    {
        var authServices = globalServerAuths
            .Values
            .SelectMany(a => a.Values)
            .Where(a => a.User.Id == user.Id);

        foreach (var a in authServices)
        {
            try
            {
                await a.LogoutAsync();
            }
            catch
            { }
        }
    }

    private readonly DbCon db;
    private readonly ILocalStorageService localStorageService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IDataProtector protector;
    private readonly int instanceId;

    public AuthService(DbCon db, ILocalStorageService localStorageService, IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtectionProvider)
    {
        this.db = db;
        this.localStorageService = localStorageService;
        this.httpContextAccessor = httpContextAccessor;
        this.protector = dataProtectionProvider.CreateProtector(ProtectionPurpose);

        instanceId = Interlocked.Increment(ref idGen);
    }

    public User User => Session?.User;
    public Session Session { get; private set; }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var protectedToken = await localStorageService.GetItemAsync<string>(TokenKeyName);
        var tokenString = protectedToken is null ? null : protector.Unprotect(protectedToken);

        ClaimsIdentity identity;

        if (!Guid.TryParse(tokenString, out var token))
        {
            identity = CreateEmptyIdentity();
        }
        else
        {
            Session = await db.Sessions.Include(a => a.User).FirstOrDefaultAsync(a => a.Token == token);
            if (Session is null || Session.Ip != IpAddress() || Session.Agent != Agent() || Session.Ened)
            {
                identity = CreateEmptyIdentity();
            }
            else
            {
                identity = new ClaimsIdentity(new[]
                     {
                    new Claim(ClaimTypes.NameIdentifier, token.ToString()),
                    new Claim(ClaimTypes.Role, "Normal"),
                    }, AuthType);

                if (!globalServerAuths.TryGetValue(token, out var auths))
                {
                    auths = new();
                    globalServerAuths.TryAdd(token, auths);
                }
                auths.TryAdd(instanceId, this);
            }
        }

        var principle = new ClaimsPrincipal(identity);
        var authState = new AuthenticationState(principle);

        return authState;
    }

    private ClaimsIdentity CreateEmptyIdentity()
    {
        Session = null;
        return new ClaimsIdentity();
    }

    public async Task CreateDefaultsAsync()
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        db.Users.Add(new()
        {
            Id = Guid.NewGuid(),
            HashedPassword = Hasher.Hash("TeWqHmQgU9DxG6#Bi"),
            Username = "MarkThompson",
        });
        await db.SaveChangesAsync();
    }

    public async Task<bool> TryLoginAsync(string username, string hash)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        username = username.ToLower();
        var user = await db.Users.FirstOrDefaultAsync(a => a.Username.ToLower() == username && a.HashedPassword == hash);

        if (user is null)
        {
            await localStorageService.RemoveItemAsync(TokenKeyName);
            return false;
        }

        var session = new Session
        {
            Token = Guid.NewGuid(),
            UserId = user.Id,
            Ip = IpAddress(),
            Agent = Agent(),
        };

        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var protectedToken = protector.Protect(session.Token.ToString());
        await localStorageService.SetItemAsync(TokenKeyName, protectedToken);

        StateChanged();

        return true;
    }

    private string IpAddress()
    {
        return httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    private string Agent()
    {
        return httpContextAccessor.HttpContext?.Request?.Headers?.UserAgent;
    }

    private void StateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public async Task LogoutAsync()
    {
        await localStorageService.RemoveItemAsync(TokenKeyName);

        if (Session is null)
        {
            return;
        }

        var token = Session.Token;

        if (globalServerAuths.TryGetValue(token, out var authServices))
        {
            foreach (var a in authServices.Values)
            {
                a.StateChanged();
            }

            authServices.Clear();
            globalServerAuths.TryRemove(token, out _);

            Session.Ened = true;
            await db.SaveChangesAsync();
        }
    }

    public void Dispose()
    {
        if (Session is not null
            &&
            globalServerAuths.TryGetValue(Session.Token, out var authServices))
        {
            authServices.TryRemove(instanceId, out _);
        }
    }
}
