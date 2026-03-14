using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Contracts.Locations.Create;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Implementations.Locations;

public sealed class LocationService : ILocationsService
{
    private readonly ILocationsRepository _locationsRepository;

    public LocationService(ILocationsRepository locationsRepository)
    {
        _locationsRepository = locationsRepository;
    }

    public async Task<Result<Guid>> CreateAsync(CreateLocationRequest location, CancellationToken cancellationToken)
    {
        var locationAddress = LocationAddress.Create(location.address.country, location.address.city, location.address.street, location.address.houseNumber);
        if (locationAddress.IsFailure)
            return Result.Failure<Guid>(locationAddress.Error);

        var locationTimezone = LocationTimeZone.Create(location.timezone);
        if (locationTimezone.IsFailure)
            return Result.Failure<Guid>(locationTimezone.Error);

        var locationName = LocationName.Create(location.name);
        if (locationName.IsFailure)
            return Result.Failure<Guid>(locationName.Error);

        var result = Location.Create(locationName.Value, locationAddress.Value, locationTimezone.Value);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        var createResult = await _locationsRepository.CreateAsync(result.Value, cancellationToken);
        if (createResult.IsFailure)
            return Result.Failure<Guid>(createResult.Error);

        return Result.Success(createResult.Value);
    }
}
