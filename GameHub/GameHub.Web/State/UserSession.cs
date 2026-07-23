using GameHub.Web.Contracts.Auth;

namespace GameHub.Web.State;

public sealed class UserSession
{
    private readonly TimeProvider _timeProvider;

    public UserSession(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public string? AccessToken { get; private set; }

    public DateTimeOffset? ExpiresAt { get; private set; }

    public AuthenticatedUser? User { get; private set; }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(AccessToken)
        && User is not null
        && ExpiresAt is not null
        && ExpiresAt > _timeProvider.GetUtcNow();

    public void Start(LoginResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (string.IsNullOrWhiteSpace(response.Token))
        {
            throw new ArgumentException(
                "A valid access token is required.",
                nameof(response));
        }

        if (response.User is null
            || string.IsNullOrWhiteSpace(response.User.Id)
            || string.IsNullOrWhiteSpace(response.User.UserName))
        {
            throw new ArgumentException(
                "A valid authenticated user is required.",
                nameof(response));
        }

        var expiresAt = new DateTimeOffset(
            response.ExpiresAt.ToUniversalTime());

        if (expiresAt <= _timeProvider.GetUtcNow())
        {
            throw new ArgumentException(
                "The authenticated session has already expired.",
                nameof(response));
        }

        AccessToken = response.Token;
        ExpiresAt = expiresAt;
        User = response.User;
    }

    public string? GetValidAccessToken()
    {
        if (IsAuthenticated)
        {
            return AccessToken;
        }

        Clear();

        return null;
    }

    public void Clear()
    {
        AccessToken = null;
        ExpiresAt = null;
        User = null;
    }
}