namespace GameHub.Web.Contracts.Auth;

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }

    public AuthenticatedUser User { get; init; }
        = new();
}