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
    private readonly IUserSessionStorage _sessionStorage;
    private Task<bool>? _restoreSessionTask;

    public AuthenticationService(
        IApiClient apiClient,
        UserSession userSession,
        IUserSessionStorage sessionStorage,
        ILogger<AuthenticationService> logger)
    {
        _apiClient = apiClient;
        _userSession = userSession;
        _sessionStorage = sessionStorage;
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

            try
            {
                await _sessionStorage.SaveAsync(result.Value!);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "The authenticated session could not be persisted.");
            }

            _restoreSessionTask = Task.FromResult(true);

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

    public async Task LogoutAsync()
    {
        _userSession.Clear();
        _restoreSessionTask = Task.FromResult(false);

        try
        {
            await _sessionStorage.ClearAsync();
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "The persisted authentication session could not be removed.");
        }
    }

    public Task<bool> RestoreSessionAsync()
    {
        return _restoreSessionTask ??= RestoreSessionCoreAsync();
    }

    private async Task<bool> RestoreSessionCoreAsync()
    {
        try
        {
            var storedSession =
                await _sessionStorage.LoadAsync();

            if (storedSession is null)
            {
                return false;
            }

            _userSession.Start(storedSession);

            if (_userSession.IsAuthenticated)
            {
                return true;
            }

            _userSession.Clear();
            await _sessionStorage.ClearAsync();

            return false;
        }
        catch (Exception exception)
        {
            _userSession.Clear();

            _logger.LogWarning(
                exception,
                "The stored user session could not be restored.");

            try
            {
                await _sessionStorage.ClearAsync();
            }
            catch (Exception clearException)
            {
                _logger.LogWarning(
                    clearException,
                    "The invalid stored user session could not be removed.");
            }

            return false;
        }
    }
}