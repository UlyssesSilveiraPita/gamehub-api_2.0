using System.Net;
using FluentAssertions;
using GameHub.Web.Contracts.Auth;
using GameHub.Web.Contracts.Common;
using GameHub.Web.Services.Authentication;
using GameHub.Web.State;
using GameHub.Web.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GameHub.Web.Tests.Unit.Services;

public class AuthenticationServiceTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 7, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task LoginAsync_ShouldStartSession_WhenLoginSucceeds()
    {
        var loginResponse = CreateLoginResponse();

        var apiClient = new StubApiClient
        {
            PostHandler = (uri, _, _) =>
            {
                uri.Should().Be("api/Auth/Login");

                return ApiResult<LoginResponse>.Success(
                    loginResponse);
            }
        };

        var session = CreateSession();

        var storage = new StubUserSessionStorage();

        var service = new AuthenticationService(
            apiClient,
            session,
            storage,
            NullLogger<AuthenticationService>.Instance);

        var result = await service.LoginAsync(
            new LoginRequest
            {
                Username = "admin@gamehub.com",
                Password = "password"
            });

        result.IsSuccess.Should().BeTrue();
        session.IsAuthenticated.Should().BeTrue();
        session.AccessToken.Should().Be("jwt-token");
        session.User!.IsInRole("Admin").Should().BeTrue();

        storage.StoredSession.Should().BeSameAs(
            loginResponse);
    }

    [Fact]
    public async Task LoginAsync_ShouldNotStartSession_WhenLoginFails()
    {
        var apiClient = new StubApiClient
        {
            PostHandler = (_, _, _) =>
                ApiResult<LoginResponse>.Failure(
                    new ApiError
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Code = "auth.invalid_credentials",
                        Message = "Invalid username or password."
                    })
        };

        var session = CreateSession();

        var service = new AuthenticationService(
            apiClient,
            session,
            new StubUserSessionStorage(),
            NullLogger<AuthenticationService>.Instance);

        var result = await service.LoginAsync(
            new LoginRequest());

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(
            "auth.invalid_credentials");
        session.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenTokenIsMissing()
    {
        var apiClient = new StubApiClient
        {
            PostHandler = (_, _, _) =>
                ApiResult<LoginResponse>.Success(
                    new LoginResponse
                    {
                        Token = string.Empty,
                        ExpiresAt = Now.AddHours(2).UtcDateTime,
                        User = new AuthenticatedUser
                        {
                            Id = "admin-1",
                            UserName = "admin@gamehub.com"
                        }
                    })
        };

        var session = CreateSession();

        var service = new AuthenticationService(
            apiClient,
            session,
            new StubUserSessionStorage(),
            NullLogger<AuthenticationService>.Instance);

        var result = await service.LoginAsync(
            new LoginRequest());

        result.IsFailure.Should().BeTrue();
        result.Error!.StatusCode.Should().Be(
            HttpStatusCode.BadGateway);
        result.Error.Code.Should().Be(
            "auth.invalid_login_response");
        session.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task Logout_ShouldClearAuthenticatedSession()
    {
        var loginResponse = CreateLoginResponse();

        var session = CreateSession();
        session.Start(loginResponse);

        var storage = new StubUserSessionStorage(
            loginResponse);

        var service = new AuthenticationService(
            new StubApiClient(),
            session,
            storage,
            NullLogger<AuthenticationService>.Instance);

        await service.LogoutAsync();

        session.IsAuthenticated.Should().BeFalse();
        session.AccessToken.Should().BeNull();
        session.ExpiresAt.Should().BeNull();
        session.User.Should().BeNull();
        storage.StoredSession.Should().BeNull();
    }

    [Fact]
    public async Task RestoreSessionAsync_ShouldRestoreValidSession()
    {
        var loginResponse = CreateLoginResponse();

        var storage = new StubUserSessionStorage(
            loginResponse);

        var session = CreateSession();

        var service = new AuthenticationService(
            new StubApiClient(),
            session,
            storage,
            NullLogger<AuthenticationService>.Instance);

        var restored =
            await service.RestoreSessionAsync();

        restored.Should().BeTrue();
        session.IsAuthenticated.Should().BeTrue();
        session.AccessToken.Should().Be("jwt-token");
        session.User.Should().NotBeNull();
        session.User!.IsInRole("Admin").Should().BeTrue();
    }

    [Fact]
    public async Task RestoreSessionAsync_ShouldRejectExpiredSession()
    {
        var expiredResponse = new LoginResponse
        {
            Token = "expired-token",
            ExpiresAt = Now.AddMinutes(-1).UtcDateTime,
            User = new AuthenticatedUser
            {
                Id = "admin-1",
                UserName = "admin@gamehub.com",
                Roles = new[]
                {
                    "Admin"
                }
            }
        };

        var storage = new StubUserSessionStorage(
            expiredResponse);

        var session = CreateSession();

        var service = new AuthenticationService(
            new StubApiClient(),
            session,
            storage,
            NullLogger<AuthenticationService>.Instance);

        var restored =
            await service.RestoreSessionAsync();

        restored.Should().BeFalse();
        session.IsAuthenticated.Should().BeFalse();
        session.AccessToken.Should().BeNull();
        session.User.Should().BeNull();
        storage.StoredSession.Should().BeNull();
    }

    [Fact]
    public async Task RestoreSessionAsync_ShouldLoadStoredSessionOnlyOnce()
    {
        var storage = new StubUserSessionStorage(
            CreateLoginResponse());

        var session = CreateSession();

        var service = new AuthenticationService(
            new StubApiClient(),
            session,
            storage,
            NullLogger<AuthenticationService>.Instance);

        var firstRestore =
            await service.RestoreSessionAsync();

        var secondRestore =
            await service.RestoreSessionAsync();

        firstRestore.Should().BeTrue();
        secondRestore.Should().BeTrue();
        storage.LoadCallCount.Should().Be(1);
        session.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public async Task RestoreSessionAsync_ShouldClearStorage_WhenStoredSessionCannotBeRead()
    {
        var storage = new StubUserSessionStorage(
            CreateLoginResponse())
        {
            LoadException = new InvalidOperationException(
                "Corrupted stored session.")
        };

        var session = CreateSession();

        var service = new AuthenticationService(
            new StubApiClient(),
            session,
            storage,
            NullLogger<AuthenticationService>.Instance);

        var restored =
            await service.RestoreSessionAsync();

        restored.Should().BeFalse();
        session.IsAuthenticated.Should().BeFalse();
        session.AccessToken.Should().BeNull();
        session.User.Should().BeNull();

        storage.LoadCallCount.Should().Be(1);
        storage.ClearCallCount.Should().Be(1);
        storage.StoredSession.Should().BeNull();
    }

    private static UserSession CreateSession()
    {
        return new UserSession(
            new StubTimeProvider(Now));
    }

    private static LoginResponse CreateLoginResponse()
    {
        return new LoginResponse
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
        };
    }
}