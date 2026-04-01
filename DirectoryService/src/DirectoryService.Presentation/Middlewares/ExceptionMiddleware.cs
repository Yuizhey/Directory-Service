using System;
using System.Security.Authentication;
using Shared.Errors;

namespace DirectoryService.Presentation.Middlewares;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred.");
        (int statusCode, Error error) = exception switch
        {
            BadHttpRequestException badRequestException => (StatusCodes.Status400BadRequest, Error.BadRequest(badRequestException.Message)),
            AuthenticationException authenticationException => (StatusCodes.Status401Unauthorized, Error.BadRequest(authenticationException.Message)),
            InvalidOperationException e => (StatusCodes.Status400BadRequest, Error.BadRequest(e.Message)),
            KeyNotFoundException e => (StatusCodes.Status404NotFound, Error.NotFound(e.Message)),
            ArgumentException e => (StatusCodes.Status400BadRequest, Error.BadRequest(e.Message)),
            NotImplementedException e => (StatusCodes.Status501NotImplemented, Error.InternalServerError("Not implemented")),
            _ => (StatusCodes.Status500InternalServerError, Error.InternalServerError("An unexpected error occurred."))
        };

        var envelope = Envelope.Failure(error);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(envelope);
    }
}
