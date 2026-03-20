using System;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Shared.Errors;

namespace DirectoryService.Application.Abstractions.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid, Failure>> CreateAsync(Location location, CancellationToken cancellationToken);
}
