using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GameHub.Web.Services.Api;
using GameHub.Web.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GameHub.Web.Tests.Unit.Services;

public class ApiClientTests
{
    [Fact]
    public async Task GetAsync_ShouldReturnSuccess_WhenApiReturnsJson()
    {
        var handler = new StubHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(
                        new
                        {
                            id = 1,
                            name = "GameHub"
                        })
                };

                return Task.FromResult(response);
            });

        var apiClient = CreateApiClient(handler);

        var result = await apiClient.GetAsync<TestResponse>(
            "api/test");

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(1);
        result.Value.Name.Should().Be("GameHub");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnString_WhenApiReturnsPlainText()
    {
        var handler = new StubHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = new StringContent("Healthy")
                };

                return Task.FromResult(response);
            });

        var apiClient = CreateApiClient(handler);

        var result = await apiClient.GetAsync<string>(
            "health");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Healthy");
    }

    [Fact]
    public async Task GetAsync_ShouldReadCodeAndMessage_WhenApiReturnsDomainError()
    {
        var handler = new StubHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(
                    HttpStatusCode.NotFound)
                {
                    Content = JsonContent.Create(
                        new
                        {
                            code = "purchase.not_found",
                            message = "Purchase was not found."
                        })
                };

                return Task.FromResult(response);
            });

        var apiClient = CreateApiClient(handler);

        var result = await apiClient.GetAsync<TestResponse>(
            "api/purchases/999");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error!.StatusCode.Should().Be(
            HttpStatusCode.NotFound);
        result.Error.Code.Should().Be("purchase.not_found");
        result.Error.Message.Should().Be(
            "Purchase was not found.");
    }

    [Fact]
    public async Task PostAsync_ShouldReadValidationErrors_WhenValidationFails()
    {
        var handler = new StubHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(
                    HttpStatusCode.BadRequest)
                {
                    Content = JsonContent.Create(
                        new
                        {
                            message = "Validation failed.",
                            errors = new[]
                            {
                                "Username is required.",
                                "Password must contain at least 6 characters."
                            }
                        })
                };

                return Task.FromResult(response);
            });

        var apiClient = CreateApiClient(handler);

        var result = await apiClient
            .PostAsync<LoginRequest, LoginResponse>(
                "api/Auth/Login",
                new LoginRequest());

        result.IsFailure.Should().BeTrue();
        result.Error!.StatusCode.Should().Be(
            HttpStatusCode.BadRequest);
        result.Error.Message.Should().Be(
            "Validation failed.");
        result.Error.ValidationErrors.Should().BeEquivalentTo(
            new[]
            {
                "Username is required.",
                "Password must contain at least 6 characters."
            });
    }

    [Fact]
    public async Task GetAsync_ShouldReadTraceId_WhenMiddlewareReturnsError()
    {
        var handler = new StubHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(
                    HttpStatusCode.InternalServerError)
                {
                    Content = JsonContent.Create(
                        new
                        {
                            success = false,
                            status = 500,
                            message = "An unexpected error occurred.",
                            traceId = "trace-123"
                        })
                };

                return Task.FromResult(response);
            });

        var apiClient = CreateApiClient(handler);

        var result = await apiClient.GetAsync<TestResponse>(
            "api/test");

        result.IsFailure.Should().BeTrue();
        result.Error!.StatusCode.Should().Be(
            HttpStatusCode.InternalServerError);
        result.Error.Message.Should().Be(
            "An unexpected error occurred.");
        result.Error.TraceId.Should().Be("trace-123");
    }

    [Fact]
    public async Task PostAsync_ShouldReadMessage_WhenApiReturnsPlainTextError()
    {
        var handler = new StubHttpMessageHandler(
            (_, _) =>
            {
                var response = new HttpResponseMessage(
                    HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(
                        "Usuario ou senha Invalidos")
                };

                return Task.FromResult(response);
            });

        var apiClient = CreateApiClient(handler);

        var result = await apiClient
            .PostAsync<LoginRequest, LoginResponse>(
                "api/Auth/Login",
                new LoginRequest());

        result.IsFailure.Should().BeTrue();
        result.Error!.StatusCode.Should().Be(
            HttpStatusCode.Unauthorized);
        result.Error.Message.Should().Be(
            "Usuario ou senha Invalidos");
    }

    [Fact]
    public async Task PostAsync_ShouldSerializeRequestAndReadResponse()
    {
        HttpMethod? capturedMethod = null;
        string? capturedContent = null;

        var handler = new StubHttpMessageHandler(
            async (request, cancellationToken) =>
            {
                capturedMethod = request.Method;
                capturedContent = await request.Content!
                    .ReadAsStringAsync(cancellationToken);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(
                        new
                        {
                            token = "jwt-token"
                        })
                };
            });

        var apiClient = CreateApiClient(handler);

        var result = await apiClient
            .PostAsync<LoginRequest, LoginResponse>(
                "api/Auth/Login",
                new LoginRequest
                {
                    Username = "ulysses",
                    Password = "password"
                });

        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().Be("jwt-token");

        capturedMethod.Should().Be(HttpMethod.Post);
        capturedContent.Should().Contain(
            "\"username\":\"ulysses\"");
        capturedContent.Should().Contain(
            "\"password\":\"password\"");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnFailure_WhenApiIsUnavailable()
    {
        var handler = new StubHttpMessageHandler(
            (_, _) => Task.FromException<HttpResponseMessage>(
                new HttpRequestException(
                    "Connection refused.")));

        var apiClient = CreateApiClient(handler);

        var result = await apiClient.GetAsync<TestResponse>(
            "api/test");

        result.IsFailure.Should().BeTrue();
        result.Error!.StatusCode.Should().Be(
            HttpStatusCode.ServiceUnavailable);
        result.Error.Code.Should().Be(
            "client.connection_failed");
        result.Error.Message.Should().Be(
            "The GameHub service is currently unavailable.");
    }

    private static ApiClient CreateApiClient(
        HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        return new ApiClient(
            httpClient,
            NullLogger<ApiClient>.Instance);
    }

    private sealed class TestResponse
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }

    private sealed class LoginRequest
    {
        public string Username { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }

    private sealed class LoginResponse
    {
        public string Token { get; init; } = string.Empty;
    }
}