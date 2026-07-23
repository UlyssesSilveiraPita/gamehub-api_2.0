using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GameHub.Web.Contracts.Common;
using GameHub.Web.Services.Abstractions;

namespace GameHub.Web.Services.Api;

public sealed class ApiClient : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(
        HttpClient httpClient,
        ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public Task<ApiResult<TResponse>> GetAsync<TResponse>(
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            requestUri);

        return SendAsync<TResponse>(
            request,
            cancellationToken);
    }

    public Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            requestUri)
        {
            Content = JsonContent.Create(
                request,
                options: JsonOptions)
        };

        return SendAsync<TResponse>(
            httpRequest,
            cancellationToken);
    }

    private async Task<ApiResult<TResponse>> SendAsync<TResponse>(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using (request)
        {
            try
            {
                using var response = await _httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await CreateApiErrorAsync(
                        response,
                        cancellationToken);

                    return ApiResult<TResponse>.Failure(error);
                }

                if (typeof(TResponse) == typeof(string))
                {
                    var text = await response.Content.ReadAsStringAsync(
                        cancellationToken);

                    return ApiResult<TResponse>.Success(
                        (TResponse)(object)text);
                }

                var value = await response.Content
                    .ReadFromJsonAsync<TResponse>(
                        JsonOptions,
                        cancellationToken);

                if (value is null)
                {
                    return ApiResult<TResponse>.Failure(
                        CreateClientError(
                            HttpStatusCode.BadGateway,
                            "client.empty_response",
                            "The API returned an empty response."));
                }

                return ApiResult<TResponse>.Success(value);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (TaskCanceledException exception)
            {
                _logger.LogWarning(
                    exception,
                    "The API request timed out. Method: {Method} | URI: {Uri}",
                    request.Method,
                    request.RequestUri);

                return ApiResult<TResponse>.Failure(
                    CreateClientError(
                        HttpStatusCode.RequestTimeout,
                        "client.request_timeout",
                        "The request took too long to complete."));
            }
            catch (HttpRequestException exception)
            {
                _logger.LogWarning(
                    exception,
                    "The API request failed. Method: {Method} | URI: {Uri}",
                    request.Method,
                    request.RequestUri);

                return ApiResult<TResponse>.Failure(
                    CreateClientError(
                        HttpStatusCode.ServiceUnavailable,
                        "client.connection_failed",
                        "The GameHub service is currently unavailable."));
            }
            catch (JsonException exception)
            {
                _logger.LogError(
                    exception,
                    "The API response could not be read. Method: {Method} | URI: {Uri}",
                    request.Method,
                    request.RequestUri);

                return ApiResult<TResponse>.Failure(
                    CreateClientError(
                        HttpStatusCode.BadGateway,
                        "client.invalid_response",
                        "The service returned an invalid response."));
            }
        }
    }

    private static async Task<ApiError> CreateApiErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
        {
            return CreateClientError(
                response.StatusCode,
                null,
                response.ReasonPhrase ?? "The request failed.");
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.String)
            {
                return CreateClientError(
                    response.StatusCode,
                    null,
                    root.GetString() ?? "The request failed.");
            }

            if (root.ValueKind == JsonValueKind.Array)
            {
                var errors = ReadErrors(root);

                return CreateClientError(
                    response.StatusCode,
                    null,
                    errors.FirstOrDefault() ?? "The request failed.",
                    validationErrors: errors);
            }

            if (root.ValueKind == JsonValueKind.Object)
            {
                var message = ReadString(root, "message")
                    ?? response.ReasonPhrase
                    ?? "The request failed.";

                var code = ReadString(root, "code");
                var traceId = ReadString(root, "traceId");

                var errors = TryGetProperty(
                        root,
                        "errors",
                        out var errorsElement)
                    ? ReadErrors(errorsElement)
                    : Array.Empty<string>();

                return CreateClientError(
                    response.StatusCode,
                    code,
                    message,
                    traceId,
                    errors);
            }
        }
        catch (JsonException)
        {
            // Some legacy endpoints return unstructured plain text.
        }

        return CreateClientError(
            response.StatusCode,
            null,
            content.Trim().Trim('"'));
    }

    private static IReadOnlyCollection<string> ReadErrors(
        JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<string>();
        }

        var errors = new List<string>();

        foreach (var item in element.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.String)
            {
                var value = item.GetString();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    errors.Add(value);
                }

                continue;
            }

            if (item.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            var description = ReadString(item, "description")
                ?? ReadString(item, "message");

            if (!string.IsNullOrWhiteSpace(description))
            {
                errors.Add(description);
            }
        }

        return errors;
    }

    private static string? ReadString(
        JsonElement element,
        string propertyName)
    {
        return TryGetProperty(
                   element,
                   propertyName,
                   out var property)
               && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static bool TryGetProperty(
        JsonElement element,
        string propertyName,
        out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(
                    property.Name,
                    propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static ApiError CreateClientError(
        HttpStatusCode statusCode,
        string? code,
        string message,
        string? traceId = null,
        IReadOnlyCollection<string>? validationErrors = null)
    {
        return new ApiError
        {
            StatusCode = statusCode,
            Code = code,
            Message = message,
            TraceId = traceId,
            ValidationErrors = validationErrors
                ?? Array.Empty<string>()
        };
    }
}