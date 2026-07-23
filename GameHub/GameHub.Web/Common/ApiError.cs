using System.Net;

namespace GameHub.Web.Contracts.Common;

public sealed class ApiError
{
    public HttpStatusCode StatusCode { get; init; }

    public string? Code { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? TraceId { get; init; }

    public IReadOnlyCollection<string> ValidationErrors { get; init; }
        = Array.Empty<string>();
}
