using System;
using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments.Update.Response;
using DirectoryService.Domain.Departments;
using Shared.Errors;

namespace DirectoryService.Application.Abstractions.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Guid, Failure>> Create(Department department, CancellationToken cancellationToken);

    Task<Result<List<Department>, Failure>> GetByIds(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<Result<List<DepartmentStructureInfo>, Failure>> GetByIdsForUpdate(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<Result<Department, Failure>> GetById(Guid id, CancellationToken cancellationToken);
    
    Task<UnitResult<Failure>> DeleteAllLocationsByDepartmentId(Guid departmentId, CancellationToken cancellationToken);

    Task<UnitResult<Failure>> MoveDepartment(Guid departmentId, Guid? parentId, CancellationToken cancellationToken);
}
