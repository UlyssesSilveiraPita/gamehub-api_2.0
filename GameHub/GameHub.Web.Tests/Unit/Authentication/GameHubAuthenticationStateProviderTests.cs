using FluentAssertions;
using GameHub.Web.Services.Authentication;
using GameHub.Web.State;
using GameHub.Web.Tests.Helpers;
using System.Security.Claims;
using GameHub.Web.Contracts.Auth;

namespace GameHub.Web.Tests.Unit.Authentication;

public class GameHubAuthenticationStateProviderTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 7, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task GetAuthenticationStateAsync_ShouldReturnAnonymousUser_WhenSessionIsEmpty()
    {
        var session = new UserSession(
            new StubTimeProvider(Now));

        using var provider =
            new GameHubAuthenticationStateProvider(
                session);

        var authenticationState =
            await provider.GetAuthenticationStateAsync();

        authenticationState.User.Identity
            .Should()
            .NotBeNull();

        authenticationState.User.Identity!
            .IsAuthenticated
            .Should()
            .BeFalse();

        authenticationState.User.Claims
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task GetAuthenticationStateAsync_ShouldCreateAuthenticatedUser_WhenSessionIsValid()
    {
        var session = new UserSession(
            new StubTimeProvider(Now));

        session.Start(
            new LoginResponse
            {
                Token = "jwt-token",
                ExpiresAt = Now.AddHours(2).UtcDateTime,
                User = new AuthenticatedUser
                {
                    Id = "admin-1",
                    UserName = "admin@gamehub.com",
                    Email = "admin@gamehub.com",
                    Roles = new[]
                    {
                        "Admin"
                    }
                }
            });

        using var provider =
            new GameHubAuthenticationStateProvider(
                session);

        var authenticationState =
            await provider.GetAuthenticationStateAsync();

        authenticationState.User.Identity
            .Should()
            .NotBeNull();

        authenticationState.User.Identity!
            .IsAuthenticated
            .Should()
            .BeTrue();

        authenticationState.User.Identity.Name
            .Should()
            .Be("admin@gamehub.com");

        authenticationState.User
            .FindFirstValue(ClaimTypes.NameIdentifier)
            .Should()
            .Be("admin-1");

        authenticationState.User
            .FindFirstValue(ClaimTypes.Email)
            .Should()
            .Be("admin@gamehub.com");

        authenticationState.User
            .IsInRole("Admin")
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task SessionChanged_ShouldNotifyAuthenticatedState_WhenSessionStarts()
    {
        var session = new UserSession(
            new StubTimeProvider(Now));

        using var provider =
            new GameHubAuthenticationStateProvider(
                session);

        Task<Microsoft.AspNetCore.Components.Authorization.AuthenticationState>?
            notifiedState = null;

        provider.AuthenticationStateChanged +=
            state => notifiedState = state;

        session.Start(
            new LoginResponse
            {
                Token = "jwt-token",
                ExpiresAt = Now.AddHours(2).UtcDateTime,
                User = new AuthenticatedUser
                {
                    Id = "admin-1",
                    UserName = "admin@gamehub.com",
                    Email = "admin@gamehub.com",
                    Roles = new[]
                    {
                        "Admin"
                    }
                }
            });

        notifiedState.Should().NotBeNull();

        var authenticationState =
            await notifiedState!;

        authenticationState.User.Identity!
            .IsAuthenticated
            .Should()
            .BeTrue();

        authenticationState.User
            .IsInRole("Admin")
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task SessionChanged_ShouldNotifyAnonymousState_WhenSessionClears()
    {
        var session = new UserSession(
            new StubTimeProvider(Now));

        session.Start(
            new LoginResponse
            {
                Token = "jwt-token",
                ExpiresAt = Now.AddHours(2).UtcDateTime,
                User = new AuthenticatedUser
                {
                    Id = "admin-1",
                    UserName = "admin@gamehub.com",
                    Email = "admin@gamehub.com",
                    Roles = new[]
                    {
                        "Admin"
                    }
                }
            });

        using var provider =
            new GameHubAuthenticationStateProvider(
                session);

        Task<Microsoft.AspNetCore.Components.Authorization.AuthenticationState>?
            notifiedState = null;

        provider.AuthenticationStateChanged +=
            state => notifiedState = state;

        session.Clear();

        notifiedState.Should().NotBeNull();

        var authenticationState =
            await notifiedState!;

        authenticationState.User.Identity
            .Should()
            .NotBeNull();

        authenticationState.User.Identity!
            .IsAuthenticated
            .Should()
            .BeFalse();

        authenticationState.User.Claims
            .Should()
            .BeEmpty();
    }
}