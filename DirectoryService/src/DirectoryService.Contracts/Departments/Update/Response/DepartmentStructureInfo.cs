using System;

namespace DirectoryService.Contracts.Departments.Update.Response;

public sealed record DepartmentStructureInfo
{
    public Guid Id { get; init; }

    public string Identifier { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public short Depth { get; init; }

    public bool IsActive { get; init; }
}
