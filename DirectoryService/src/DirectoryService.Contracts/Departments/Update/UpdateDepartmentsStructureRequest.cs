using System;

namespace DirectoryService.Contracts.Departments.Update;

public sealed record UpdateDepartmentsStructureRequest
{
    public required Guid? ParentId { get; init; }
}