using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Contracts.Locations.Create;
using DirectoryService.Presentation.ResponseResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public sealed class LocationsController : ControllerBase
{
    private readonly ILocationsService _locationsService;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(ILocationsService locationsService, ILogger<LocationsController> logger)
    {
        _locationsService = locationsService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<EndPointResult<Guid>> Create([FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _locationsService.CreateAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogWarning(
                "POST api/locations завершился с ошибкой: TraceId={TraceId}",
                HttpContext.TraceIdentifier);
        }

        return result;
    }
}
