using FluentAssertions;
using GameHub.Web.Contracts.Auth;
using GameHub.Web.State;
using GameHub.Web.Tests.Helpers;

namespace GameHub.Web.Tests.Unit.State;

public class UserSessionTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 7, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Start_ShouldCreateAuthenticatedSession()
    {
        var timeProvider = new StubTimeProvider(Now);
        var session = new UserSession(timeProvider);

        session.Start(CreateLoginResponse(
            Now.AddHours(2)));

        session.IsAuthenticated.Should().BeTrue();
        session.AccessToken.Should().Be("test-token");
        session.ExpiresAt.Should().Be(
            Now.AddHours(2));
        session.User.Should().NotBeNull();
        session.User!.UserName.Should().Be(
            "admin@gamehub.com");
        session.User.IsInRole("Admin").Should().BeTrue();
        session.User.IsInRole("admin").Should().BeTrue();
    }

    [Fact]
    public void GetValidAccessToken_ShouldReturnToken_WhenSessionIsValid()
    {
        var timeProvider = new StubTimeProvider(Now);
        var session = new UserSession(timeProvider);

        session.Start(CreateLoginResponse(
            Now.AddMinutes(30)));

        var token = session.GetValidAccessToken();

        token.Should().Be("test-token");
        session.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void GetValidAccessToken_ShouldClearSession_WhenTokenExpired()
    {
        var timeProvider = new StubTimeProvider(Now);
        var session = new UserSession(timeProvider);

        session.Start(CreateLoginResponse(
            Now.AddMinutes(5)));

        timeProvider.Advance(TimeSpan.FromMinutes(6));

        var token = session.GetValidAccessToken();

        token.Should().BeNull();
        session.IsAuthenticated.Should().BeFalse();
        session.AccessToken.Should().BeNull();
        session.ExpiresAt.Should().BeNull();
        session.User.Should().BeNull();
    }

    [Fact]
    public void Start_ShouldRejectSession_WhenAlreadyExpired()
    {
        var timeProvider = new StubTimeProvider(Now);
        var session = new UserSession(timeProvider);

        var action = () => session.Start(
            CreateLoginResponse(
                Now.AddMinutes(-1)));

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage(
                "The authenticated session has already expired.*");

        session.IsAuthenticated.Should().BeFalse();
        session.AccessToken.Should().BeNull();
        session.ExpiresAt.Should().BeNull();
        session.User.Should().BeNull();
    }

    [Fact]
    public void Start_ShouldRejectSession_WhenUserIsInvalid()
    {
        var timeProvider = new StubTimeProvider(Now);
        var session = new UserSession(timeProvider);

        var response = new LoginResponse
        {
            Token = "test-token",
            ExpiresAt = Now.AddHours(1).UtcDateTime,
            User = new AuthenticatedUser
            {
                Id = "admin-1",
                UserName = string.Empty,
                Email = "admin@gamehub.com",
                Roles = new[]
                {
                    "Admin"
                }
            }
        };

        var action = () => session.Start(response);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage(
                "A valid authenticated user is required.*");

        session.IsAuthenticated.Should().BeFalse();
        session.AccessToken.Should().BeNull();
        session.ExpiresAt.Should().BeNull();
        session.User.Should().BeNull();
    }

    [Fact]
    public void Clear_ShouldRemoveAuthenticatedSession()
    {
        var timeProvider = new StubTimeProvider(Now);
        var session = new UserSession(timeProvider);

        session.Start(CreateLoginResponse(
            Now.AddHours(1)));

        session.Clear();

        session.IsAuthenticated.Should().BeFalse();
        session.AccessToken.Should().BeNull();
        session.ExpiresAt.Should().BeNull();
        session.User.Should().BeNull();
    }

    [Fact]
    public void Start_ShouldPreserveCurrentSession_WhenNewSessionIsInvalid()
    {
        var timeProvider = new StubTimeProvider(Now);
        var session = new UserSession(timeProvider);

        var validResponse = CreateLoginResponse(
            Now.AddHours(2));

        session.Start(validResponse);

        var invalidResponse = new LoginResponse
        {
            Token = "new-token",
            ExpiresAt = Now.AddMinutes(-1).UtcDateTime,
            User = new AuthenticatedUser
            {
                Id = "another-user",
                UserName = "player@gamehub.com",
                Email = "player@gamehub.com",
                Roles = Array.Empty<string>()
            }
        };

        var action = () => session.Start(invalidResponse);

        action.Should().Throw<ArgumentException>();

        session.IsAuthenticated.Should().BeTrue();
        session.AccessToken.Should().Be("test-token");
        session.ExpiresAt.Should().Be(Now.AddHours(2));
        session.User.Should().BeSameAs(validResponse.User);
    }

    private static LoginResponse CreateLoginResponse(
        DateTimeOffset expiresAt)
    {
        return new LoginResponse
        {
            Token = "test-token",
            ExpiresAt = expiresAt.UtcDateTime,
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