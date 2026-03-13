using System;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Abstractions.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid>> CreateAsync(Location location, CancellationToken cancellationToken);
}
