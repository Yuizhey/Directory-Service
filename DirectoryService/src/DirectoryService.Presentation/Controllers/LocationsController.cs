using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Contracts.Locations.Create;
using DirectoryService.Presentation.Extensions;
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
    public async Task<IActionResult> Create([FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _locationsService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToResponse();
    }
}
