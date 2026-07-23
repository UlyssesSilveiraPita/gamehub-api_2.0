using GameHub.Web.Contracts.Auth;

namespace GameHub.Web.Services.Abstractions;

public interface IUserSessionStorage
{
    Task SaveAsync(LoginResponse response);

    Task<LoginResponse?> LoadAsync();

    Task ClearAsync();
}