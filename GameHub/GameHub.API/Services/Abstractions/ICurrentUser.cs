namespace GameHub.API.Services.Abstractions;

public interface ICurrentUser
{
    string? UserId { get; }

    string? Email { get; }

    bool IsAuthenticated { get; }
}
