using GameHub.Web.Services.Abstractions;

namespace GameHub.Web.Services.Api;

public sealed class ApiHealthService : IApiHealthService
{
    private readonly IApiClient _apiClient;

    public ApiHealthService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<bool> IsHealthyAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _apiClient.GetAsync<string>(
            "health",
            cancellationToken);

        return result.IsSuccess;
    }
}