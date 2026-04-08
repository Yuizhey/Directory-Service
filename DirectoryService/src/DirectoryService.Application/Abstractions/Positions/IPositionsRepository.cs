using System;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Positions;
using Shared.Errors;

namespace DirectoryService.Application.Abstractions.Positions;

public interface IPositionsRepository
{
    Task<Result<Guid, Failure>> Create(Position position, CancellationToken cancellationToken);
}
