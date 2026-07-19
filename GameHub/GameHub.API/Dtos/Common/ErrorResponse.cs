namespace GameHub.API.Dtos.Common;

public sealed class ErrorResponse // sealed "Esta classe não foi feita para herança."
{
    public bool Success { get; init; } = false; // inicia como false pois a classe representa apenas erro.

    public int Status { get; init; }

    public string Message { get; init; } = string.Empty;

    public string TraceId { get; init; } = string.Empty;
}
