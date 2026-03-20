using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Shared.Errors;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public LocationsRepository(DirectoryServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid, Failure>> CreateAsync(Location location, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Locations.AddAsync(location, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success<Guid, Failure>(location.Id);
        }
        catch (DbUpdateException)
        {
            return Result.Failure<Guid, Failure>(Error.Conflict("An error occurred while saving the location to the database"));
        }
    }
}