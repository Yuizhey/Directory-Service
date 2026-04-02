using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<LocationsRepository> _logger;

    public LocationsRepository(DirectoryServiceDbContext dbContext, ILogger<LocationsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> CreateAsync(Location location, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Locations.AddAsync(location, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success<Guid, Failure>(location.Id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Ошибка при сохранении локации в БД (LocationId={LocationId})",
                location.Id);
            return Result.Failure<Guid, Failure>(Error.Conflict("An error occurred while saving the location to the database"));
        }
    }
}