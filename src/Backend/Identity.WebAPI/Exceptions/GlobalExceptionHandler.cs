using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Identity.WebAPI.Exceptions;

sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled server error");
        else
            logger.LogWarning(exception, "Request failed with status {StatusCode}", statusCode);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        if (environment.IsDevelopment())
            problemDetails.Detail = exception.Message;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    static (int StatusCode, string Title) MapException(Exception exception) =>
        exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            BadHttpRequestException badRequest => (badRequest.StatusCode, badRequest.Message),
            FormatException => (StatusCodes.Status400BadRequest, ApiErrors.InvalidUserIdentifier),
            _ => (StatusCodes.Status500InternalServerError, ApiErrors.UnexpectedError)
        };
}
