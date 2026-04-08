using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Positions;
using DirectoryService.Domain.Positions;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Infrastructure.Repositories;

public sealed class PositionsRepository : IPositionsRepository
{
    private readonly ILogger<PositionsRepository> _logger;
    private readonly DirectoryServiceDbContext _dbContext;

    public PositionsRepository(ILogger<PositionsRepository> logger, DirectoryServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public Task<Result<Guid, Failure>> Create(Position position, CancellationToken cancellationToken)
    {
        try
        {

        }
        catch
        {
            
        }
    }
}
