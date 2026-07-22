using GameHub.Web.Services.Abstractions;

namespace GameHub.Web.Services.Api;

public sealed class ApiHealthService : IApiHealthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiHealthService> _logger;

    public ApiHealthService(
        HttpClient httpClient,
        ILogger<ApiHealthService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> IsHealthyAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync(
                "health",
                cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(
                exception,
                "The GameHub API health check could not be reached.");

            return false;
        }
        catch (TaskCanceledException exception)
            when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                exception,
                "The GameHub API health check timed out.");

            return false;
        }
    }
}