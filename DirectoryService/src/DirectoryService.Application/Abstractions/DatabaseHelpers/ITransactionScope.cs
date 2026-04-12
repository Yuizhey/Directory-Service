using System;
using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Application.Abstractions.DatabaseHelpers;

public interface ITransactionScope : IDisposable
{
    UnitResult<Failure> Commit();

    UnitResult<Failure> Rollback();
}
