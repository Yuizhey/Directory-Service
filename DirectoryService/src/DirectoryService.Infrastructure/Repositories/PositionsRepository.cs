using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Positions;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
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
    public async Task<Result<Guid, Failure>> Create(Position position, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Positions.AddAsync(position, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success<Guid, Failure>(position.Id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Ошибка при сохранении позиции в БД (PositionId={PositionId})",
                position.Id);
            return Result.Failure<Guid, Failure>(Error.Conflict("An error occurred while saving the position to the database"));
        }
    }
}
