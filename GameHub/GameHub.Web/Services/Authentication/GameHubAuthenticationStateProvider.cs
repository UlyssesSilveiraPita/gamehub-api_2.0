using System.Security.Claims;
using GameHub.Web.State;
using Microsoft.AspNetCore.Components.Authorization;

namespace GameHub.Web.Services.Authentication;

public sealed class GameHubAuthenticationStateProvider
    : AuthenticationStateProvider, IDisposable
{
    private static readonly ClaimsPrincipal AnonymousUser =
        new(new ClaimsIdentity());

    private readonly UserSession _userSession;

    public GameHubAuthenticationStateProvider(
        UserSession userSession)
    {
        _userSession = userSession;
        _userSession.SessionChanged += HandleSessionChanged;
    }

    public override Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        return Task.FromResult(
            CreateAuthenticationState());
    }

    private AuthenticationState CreateAuthenticationState()
    {
        if (!_userSession.IsAuthenticated
            || _userSession.User is not { } user)
        {
            return new AuthenticationState(
                AnonymousUser);
        }

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                user.Id),

            new(
                ClaimTypes.Name,
                user.UserName)
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Email,
                    user.Email));
        }

        claims.AddRange(
            user.Roles.Select(
                role => new Claim(
                    ClaimTypes.Role,
                    role)));

        var identity = new ClaimsIdentity(
            claims,
            authenticationType: "GameHubJwt",
            nameType: ClaimTypes.Name,
            roleType: ClaimTypes.Role);

        return new AuthenticationState(
            new ClaimsPrincipal(identity));
    }

    private void HandleSessionChanged()
    {
        NotifyAuthenticationStateChanged(
            GetAuthenticationStateAsync());
    }

    public void Dispose()
    {
        _userSession.SessionChanged -= HandleSessionChanged;
    }
}