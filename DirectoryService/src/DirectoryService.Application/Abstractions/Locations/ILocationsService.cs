using System;
using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations.Create;

namespace DirectoryService.Application.Abstractions.Locations;

public interface ILocationsService
{
    Task<Result<Guid>> CreateAsync(CreateLocationRequest location, CancellationToken cancellationToken);
}
