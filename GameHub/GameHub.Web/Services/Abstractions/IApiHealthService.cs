namespace GameHub.Web.Services.Abstractions;

public interface IApiHealthService
{
    Task<bool> IsHealthyAsync(
        CancellationToken cancellationToken = default);
}