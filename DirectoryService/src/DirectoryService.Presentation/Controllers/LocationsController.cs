using System;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Contracts.Locations.Create;
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
    public async Task<ActionResult<Guid>> Create([FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _locationsService.CreateAsync(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
