using System;

namespace DirectoryService.Domain.Departments;

/// <summary>
/// Department and Position entities relation.
/// </summary>
public class DepartmentPosition
{
    public DepartmentPosition(Guid departmentId, Guid positionId)
    {
        Id = Guid.NewGuid();
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public Guid Id { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }
}
