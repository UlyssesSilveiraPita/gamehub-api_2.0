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

        var service = new AuthenticationService(
            apiClient,
            session,
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
    public void Logout_ShouldClearAuthenticatedSession()
    {
        var session = CreateSession();
        session.Start(CreateLoginResponse());

        var service = new AuthenticationService(
            new StubApiClient(),
            session,
            NullLogger<AuthenticationService>.Instance);

        service.Logout();

        session.IsAuthenticated.Should().BeFalse();
        session.AccessToken.Should().BeNull();
        session.User.Should().BeNull();
                
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