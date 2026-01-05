using System;

namespace DirectoryService.Domain.Departments;

/// <summary>
/// Department and Location entities relation.
/// </summary>
public class DepartmentLocation
{
    public DepartmentLocation(Guid departmentId, Guid locationId)
    {
        Id = Guid.NewGuid();
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public Guid Id { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid LocationId { get; set; }
}
