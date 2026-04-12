using System;
using Shared.Errors;

namespace DirectoryService.Presentation.ResponseResult;

public sealed class FailureResult : IResult
{
    private readonly Failure _failure;

    public FailureResult(Failure failure)
    {
        _failure = failure;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope<Failure>.Failure(_failure);
        httpContext.Response.StatusCode = GetStatusCode(_failure);
        return httpContext.Response.WriteAsJsonAsync(envelope);
    }

    private static int GetStatusCode(Failure failure)
    {
        if (!failure.Any())
        {
            return StatusCodes.Status500InternalServerError;
        }

        var errorTypesCount = failure.Select(e => e.Type).Distinct().Count();

        return errorTypesCount > 1
            ? StatusCodes.Status500InternalServerError
            : failure.First().Type switch
            {
                ErrorType.BAD_REQUEST => StatusCodes.Status400BadRequest,
                ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
                ErrorType.CONFLICT => StatusCodes.Status409Conflict,
                ErrorType.INTERNAL_SERVER_ERROR => StatusCodes.Status500InternalServerError,
                ErrorType.SERVICE_UNAVAILABLE => StatusCodes.Status503ServiceUnavailable,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}
