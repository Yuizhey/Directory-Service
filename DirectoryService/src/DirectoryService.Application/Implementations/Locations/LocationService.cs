using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Contracts.Locations.Create;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Application.Implementations.Locations;

public sealed class LocationService : ILocationsService
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<LocationService> _logger;

    public LocationService(ILocationsRepository locationsRepository, ILogger<LocationService> logger)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> CreateAsync(CreateLocationRequest location, CancellationToken cancellationToken)
    {
        var locationAddress = LocationAddress.Create(location.address.country, location.address.city, location.address.street, location.address.houseNumber);
        var errors = new List<Error>();
        if (locationAddress.IsFailure)
        {
            errors.AddRange(locationAddress.Error.Errors);
        }

        var locationTimezone = LocationTimeZone.Create(location.timezone);
        if (locationTimezone.IsFailure)
        {
            errors.AddRange(locationTimezone.Error.Errors);
        }

        var locationName = LocationName.Create(location.name);
        if (locationName.IsFailure)
        {
            errors.AddRange(locationName.Error.Errors);
        }

        if (errors.Any())
        {
            _logger.LogWarning(
                "Отклонён запрос на создание локации: ошибки валидации: {Errors}",
                string.Join("; ", errors.Select(e => $"{e.Code}: {e.Message}")));
            return Result.Failure<Guid, Failure>(errors);
        }

        var result = Location.Create(locationName.Value, locationAddress.Value, locationTimezone.Value);
        var createResult = await _locationsRepository.CreateAsync(result.Value, cancellationToken);
        if (createResult.IsFailure)
        {
            _logger.LogWarning(
                "Создание локации не завершено: {Errors}",
                string.Join("; ", createResult.Error.Select(e => $"{e.Code}: {e.Message}")));
            return Result.Failure<Guid, Failure>(createResult.Error);
        }

        _logger.LogInformation(
            "Создана локация: {LocationId}, часовой пояс: {TimeZone}",
            createResult.Value,
            location.timezone);
        return Result.Success<Guid, Failure>(createResult.Value);
    }
}
