namespace GameHub.API.Dtos.Auth;

public sealed class AuthenticatedUserResponseDto
{
    public string Id { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; }
        = Array.Empty<string>();
}