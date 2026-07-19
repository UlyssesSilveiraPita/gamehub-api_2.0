namespace GameHub.API.Dtos.Common;

public sealed class ValidationErrorResponse
{
    public string Message { get; init; } = "Validation failed.";

    public IReadOnlyCollection<string> Errors { get; init; }
        = Array.Empty<string>();
}
