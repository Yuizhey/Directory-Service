using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
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
            if(ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                if(pgEx.ConstraintName == "ix_locations_name")
                {
                    _logger.LogWarning(
                        "Попытка создать локацию с уже существующим именем (LocationId={LocationId}, LocationName={LocationName})",
                        location.Id, 
                        location.Name.Value);
                    return Result.Failure<Guid, Failure>(Error.Conflict("A location with the same name already exists"));
                }

                if(pgEx.ConstraintName == "ux_locations_address")
                {
                    _logger.LogWarning(
                        "Попытка создать локацию с уже существующим адресом (LocationId={LocationId}, LocationAddress={LocationAddress})",
                        location.Id, 
                        $"{location.Address.Country}, {location.Address.City}, {location.Address.Street}, {location.Address.HouseNumber}");
                    return Result.Failure<Guid, Failure>(Error.Conflict("A location with the same address already exists"));
                }
            }

            _logger.LogError(
                ex,
                "Ошибка при сохранении локации в БД (LocationId={LocationId})",
                location.Id);
            return Result.Failure<Guid, Failure>(Error.Conflict("An error occurred while saving the location to the database"));
        }
    }

    public async Task<Result<List<Location>, Failure>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        try
        {
            var locations = await _dbContext.Locations.Where(l => ids.Contains(l.Id)).ToListAsync(cancellationToken);
            return Result.Success<List<Location>, Failure>(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ошибка при получении локаций по идентификаторам (LocationIds={LocationIds})",
                string.Join(", ", ids));
            return Result.Failure<List<Location>, Failure>(Error.Conflict("An error occurred while retrieving locations from the database"));
        }
    }
}