using System;
using DirectoryService.Presentation.ResponseResult;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<EndPointResult<Guid>> Create(CancellationToken cancellationToken)
    {

    }
}
