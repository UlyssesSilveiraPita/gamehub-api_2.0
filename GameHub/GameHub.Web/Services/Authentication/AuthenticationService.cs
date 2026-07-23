using System.Net;
using GameHub.Web.Contracts.Auth;
using GameHub.Web.Contracts.Common;
using GameHub.Web.Services.Abstractions;
using GameHub.Web.State;

namespace GameHub.Web.Services.Authentication;

public sealed class AuthenticationService
    : IAuthenticationService
{
    private const string LoginEndpoint =
        "api/Auth/Login";

    private readonly IApiClient _apiClient;
    private readonly UserSession _userSession;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IApiClient apiClient,
        UserSession userSession,
        ILogger<AuthenticationService> logger)
    {
        _apiClient = apiClient;
        _userSession = userSession;
        _logger = logger;
    }

    public async Task<ApiResult<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await _apiClient
            .PostAsync<LoginRequest, LoginResponse>(
                LoginEndpoint,
                request,
                cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        try
        {
            _userSession.Start(result.Value!);

            return result;
        }
        catch (ArgumentException exception)
        {
            _logger.LogError(
                exception,
                "The login response did not contain a valid session.");

            return ApiResult<LoginResponse>.Failure(
                new ApiError
                {
                    StatusCode = HttpStatusCode.BadGateway,
                    Code = "auth.invalid_login_response",
                    Message =
                        "The authentication service returned an invalid response."
                });
        }
    }

    public void Logout()
    {
        _userSession.Clear();
    }
}