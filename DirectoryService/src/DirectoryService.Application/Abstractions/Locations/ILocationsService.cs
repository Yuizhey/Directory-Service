using System;
using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations.Create;
using Shared.Errors;

namespace DirectoryService.Application.Abstractions.Locations;

public interface ILocationsService
{
    Task<Result<Guid, Failure>> CreateAsync(CreateLocationRequest location, CancellationToken cancellationToken);
}
