using System;
using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Implementations.Positions.CreatePositionCommand;
using DirectoryService.Contracts.Positions.Create;
using DirectoryService.Presentation.ResponseResult;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/positions")]
public sealed class PositionsController : ControllerBase
{
    private readonly ILogger<PositionsController> _logger;

    public PositionsController(ILogger<PositionsController> logger) 
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<EndPointResult<Guid>> Create([FromServices]ICommandHandler<Guid, CreatePositionCommand> handler, [FromBody]CreatePositionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(request);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogWarning(
                "POST api/positions завершился с ошибкой: TraceId={TraceId}",
                HttpContext.TraceIdentifier);
        }

        return result;
    }
}
