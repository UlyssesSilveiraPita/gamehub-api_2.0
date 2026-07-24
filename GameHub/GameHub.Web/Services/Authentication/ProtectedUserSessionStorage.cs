using GameHub.Web.Contracts.Auth;
using GameHub.Web.Services.Abstractions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace GameHub.Web.Services.Authentication;

public sealed class ProtectedUserSessionStorage
    : IUserSessionStorage
{
    private const string StorageKey =
        "gamehub.auth.session";

    private readonly ProtectedSessionStorage _storage;

    public ProtectedUserSessionStorage(
        ProtectedSessionStorage storage)
    {
        _storage = storage;
    }

    public async Task SaveAsync(
        LoginResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        await _storage.SetAsync(
            StorageKey,
            response);
    }

    public async Task<LoginResponse?> LoadAsync()
    {
        var result = await _storage.GetAsync<LoginResponse>(
            StorageKey);

        return result.Success
            ? result.Value
            : null;
    }

    public async Task ClearAsync()
    {
        await _storage.DeleteAsync(StorageKey);
    }
}