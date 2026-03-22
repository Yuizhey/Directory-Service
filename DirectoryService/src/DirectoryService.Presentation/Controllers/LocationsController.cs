using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Contracts.Locations.Create;
using DirectoryService.Presentation.ResponseResult;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public sealed class LocationsController : ControllerBase
{
    private readonly ILocationsService _locationsService;

    public LocationsController(ILocationsService locationsService)
    {
        _locationsService = locationsService;
    }

    [HttpPost]
    public async Task<EndPointResult<Guid>> Create([FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        return await _locationsService.CreateAsync(request, cancellationToken);
    }
}
