using GameHub.Web.Contracts.Common;
using GameHub.Web.Services.Abstractions;

namespace GameHub.Web.Tests.Helpers;

internal sealed class StubApiClient : IApiClient
{
    public Func<string, CancellationToken, object>? GetHandler
    {
        get;
        init;
    }

    public Func<string, object, CancellationToken, object>? PostHandler
    {
        get;
        init;
    }

    public Task<ApiResult<TResponse>> GetAsync<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        if (GetHandler is null)
        {
            throw new InvalidOperationException(
                "No GET handler was configured.");
        }

        var result = GetHandler(
            requestUri,
            cancellationToken);

        return Task.FromResult(
            (ApiResult<TResponse>)result);
    }

    public Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        if (PostHandler is null)
        {
            throw new InvalidOperationException(
                "No POST handler was configured.");
        }

        var result = PostHandler(
            requestUri,
            request!,
            cancellationToken);

        return Task.FromResult(
            (ApiResult<TResponse>)result);
    }
}