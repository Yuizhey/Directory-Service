using System;

namespace DirectoryService.Presentation.ResponseResult;

public sealed class SuccessResult<T> : IResult
{
    private readonly T _result;

    public SuccessResult(T result)
    {
        _result = result;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope<T>.Success(_result);
        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        return httpContext.Response.WriteAsJsonAsync(envelope);
    }
}