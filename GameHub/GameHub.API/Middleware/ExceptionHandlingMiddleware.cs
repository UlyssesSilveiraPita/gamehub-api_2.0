using Microsoft.AspNetCore.Http;

namespace GameHub.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var statusCode = exception switch
        {
            ArgumentException =>
                StatusCodes.Status400BadRequest,

            InvalidOperationException =>
                StatusCodes.Status400BadRequest,

            KeyNotFoundException =>
                StatusCodes.Status404NotFound,

            UnauthorizedAccessException =>
                StatusCodes.Status403Forbidden,

            _ =>
                StatusCodes.Status500InternalServerError //Qualquer exceção que não corresponda aos casos anteriores.
        };

        var message = statusCode ==
            StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : exception.Message;

        var response = new
        {
            Success = false,
            Status = statusCode,
            Message = message,
            TraceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(response);
    }
}
