namespace GameHub.Web.Contracts.Auth;

public sealed class AuthenticatedUser
{
    public string Id { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; }
        = Array.Empty<string>();

    public bool IsInRole(string role)
    {
        return Roles.Contains(
            role,
            StringComparer.OrdinalIgnoreCase);
    }
}