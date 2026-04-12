using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Implementations.Departments.CreateDepartmentCommand;
using DirectoryService.Application.Implementations.Departments.UpdateDepartmentLocationsCommand;
using DirectoryService.Contracts.Departments.Create;
using DirectoryService.Contracts.Departments.Update;
using DirectoryService.Presentation.ResponseResult;
using Microsoft.AspNetCore.Mvc;
using Shared.Errors;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController : ControllerBase
{
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(ILogger<DepartmentsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<EndPointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateDepartmentCommand> handler,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(request);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogWarning(
                "POST api/departments завершился с ошибкой: TraceId={TraceId}",
                HttpContext.TraceIdentifier);
        }

        return result;
    }
    
    [HttpPatch("{id:guid}/locations")]
    public async Task<EndPointResult<Guid>> Update(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<UpdateDepartmentLocationsCommand> handler,
        [FromBody] UpdateDepartmentLocationsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentLocationsCommand(id, request);
        
        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogWarning(
                "POST api/departments завершился с ошибкой: TraceId={TraceId}",
                HttpContext.TraceIdentifier);
        }
        
        return Result.Success<Guid, Failure>(id);
    }
}
