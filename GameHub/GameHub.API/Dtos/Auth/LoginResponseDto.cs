namespace GameHub.API.Dtos.Auth;

public sealed class LoginResponseDto
{
    public string Token { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }

    public AuthenticatedUserResponseDto User { get; init; }
        = new();
}