using GameHub.Web.Contracts.Common;

namespace GameHub.Web.Services.Abstractions;

public interface IApiClient
{
    Task<ApiResult<TResponse>> GetAsync<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default);

    Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken = default);
}