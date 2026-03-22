using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shared.Errors;

namespace DirectoryService.Presentation.Extensions;

public static class ResponseExtension
{
    public static ActionResult ToResponse(this Failure errors)
    {
        if (!errors.Any())
        {
            return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var errorTypesCount = errors.Select(e => e.Type).Distinct().Count();

        return errorTypesCount > 1
            ? new ObjectResult(errors) { StatusCode = StatusCodes.Status500InternalServerError }
            : new ObjectResult(errors) { StatusCode = GetStatusCode(errors.First().Type) };
    }

    private static int GetStatusCode(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.BAD_REQUEST => StatusCodes.Status400BadRequest,
            ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ErrorType.CONFLICT => StatusCodes.Status409Conflict,
            ErrorType.INTERNAL_SERVER_ERROR => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
