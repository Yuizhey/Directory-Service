using System;
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
    public async Task<EndPointResult<Guid>> Create(CancellationToken cancellationToken)
    {

    }
}
