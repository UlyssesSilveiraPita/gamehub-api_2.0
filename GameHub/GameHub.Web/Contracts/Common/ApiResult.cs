namespace GameHub.Web.Contracts.Common;

public sealed class ApiResult<T>
{
    private ApiResult(
        bool isSuccess,
        T? value,
        ApiError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T? Value { get; }

    public ApiError? Error { get; }

    public static ApiResult<T> Success(T value)
    {
        return new ApiResult<T>(
            true,
            value,
            null);
    }

    public static ApiResult<T> Failure(ApiError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new ApiResult<T>(
            false,
            default,
            error);
    }
}