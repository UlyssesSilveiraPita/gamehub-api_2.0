using GameHub.Web.Contracts.Auth;
using GameHub.Web.Services.Abstractions;

namespace GameHub.Web.Tests.Helpers;

internal sealed class StubUserSessionStorage
    : IUserSessionStorage
{
    public StubUserSessionStorage(
        LoginResponse? storedSession = null)
    {
        StoredSession = storedSession;
    }

    public LoginResponse? StoredSession { get; private set; }

    public int SaveCallCount { get; private set; }

    public int LoadCallCount { get; private set; }

    public int ClearCallCount { get; private set; }

    public Exception? LoadException { get; init; }

    public Task SaveAsync(LoginResponse response)
    {
        SaveCallCount++;
        StoredSession = response;

        return Task.CompletedTask;
    }

    public Task<LoginResponse?> LoadAsync()
    {
        LoadCallCount++;

        if (LoadException is not null)
        {
            throw LoadException;
        }

        return Task.FromResult(StoredSession);
    }

    public Task ClearAsync()
    {
        ClearCallCount++;
        StoredSession = null;

        return Task.CompletedTask;
    }
}