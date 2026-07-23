using GameHub.Web.Contracts.Auth;
using GameHub.Web.Contracts.Common;

namespace GameHub.Web.Services.Abstractions;

public interface IAuthenticationService
{
    Task<ApiResult<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    void Logout();
}