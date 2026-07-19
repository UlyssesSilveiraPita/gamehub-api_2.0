namespace GameHub.API.Dtos.Common;

public sealed class ApiErrorResponse
{
    public string Code { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
