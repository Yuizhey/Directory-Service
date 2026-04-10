using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Application.Implementations.Locations.CreateLocationCommand;
using DirectoryService.Contracts.Locations.Create;
using DirectoryService.Presentation.ResponseResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public sealed class LocationsController : ControllerBase
{
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(ILogger<LocationsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<EndPointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler,
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request);
        
        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogWarning(
                "POST api/locations завершился с ошибкой: TraceId={TraceId}",
                HttpContext.TraceIdentifier);
        }

        return result;
    }
}
