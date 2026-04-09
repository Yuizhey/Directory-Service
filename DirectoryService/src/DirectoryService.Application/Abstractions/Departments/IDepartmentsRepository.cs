using System;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using Shared.Errors;

namespace DirectoryService.Application.Abstractions.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Guid, Failure>> Create(Department department, CancellationToken cancellationToken);
    
    Task<Result<List<Department>, Failure>> GetByIds(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<Result<Department, Failure>> GetById(Guid ids, CancellationToken cancellationToken);
}
