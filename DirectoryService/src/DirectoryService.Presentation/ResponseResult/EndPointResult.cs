using System;
using CSharpFunctionalExtensions;
using Shared.Errors;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Presentation.ResponseResult;

public sealed class EndPointResult<T> : IResult
{
    private readonly IResult _result;

    public EndPointResult(Result<T, Failure> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult<T>(result.Value)
            : new FailureResult(result.Error);
    }

    public Task ExecuteAsync(HttpContext httpContext) => _result.ExecuteAsync(httpContext);

    public static implicit operator EndPointResult<T>(Result<T, Failure> result) => new EndPointResult<T>(result);
}