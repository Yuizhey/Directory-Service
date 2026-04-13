using System;
using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Application.Abstractions.DatabaseHelpers;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Failure>> BeginTransactionAsync(CancellationToken cancellationToken);
    
    Task<UnitResult<Failure>> SaveChangesAsync(CancellationToken cancellationToken);
}