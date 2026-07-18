using System.Security.Claims;
using GameHub.API.Services.Abstractions;

namespace GameHub.API.Services.Authentication;

//recebe: IHttpContextAccessor Esse objeto permite acessar o HttpContext da requisição atual.\\
public sealed class CurrentUserService : ICurrentUser // sealed -> classe não foi projetada para herança.
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Email =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?
            .User
            .Identity?
            .IsAuthenticated
        ?? false;
}
