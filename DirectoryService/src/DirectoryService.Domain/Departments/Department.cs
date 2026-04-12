using System;
using System.ComponentModel;
using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Departments;

public class Department
{
    private List<Department> _children = [];
    private List<DepartmentLocation> _locations = [];
    private List<DepartmentPosition> _positions = [];

    /// <summary>
    /// Private constructor for EF Core. Do not use it directly to create Department instances.
    /// </summary>
    private Department() { }

    private Department(
        DepartmentName name,
        DepartmentIdentifier identifier,
        DepartmentPath path,
        short depth,
        bool isActive,
        IEnumerable<Guid> locationIds,
        Department? parent = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        Path = path;
        Depth = depth;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        _locations = locationIds.Select(locationId => new DepartmentLocation(this.Id, locationId)).ToList();
        Parent = parent;
    }

    public Guid Id { get; private set; }

    public DepartmentName Name { get; private set; }

    public DepartmentIdentifier Identifier { get; private set; }

    public DepartmentPath Path { get; private set; }

    public short Depth { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public Department? Parent { get; private set; }

    public IReadOnlyList<Department> Children => _children.AsReadOnly();

    public IReadOnlyList<DepartmentLocation> Locations => _locations.AsReadOnly();

    public IReadOnlyList<DepartmentPosition> Positions => _positions.AsReadOnly();

    public static Result<Department> Create(DepartmentName name, DepartmentIdentifier identifier, DepartmentPath path, short depth, bool isActive, IEnumerable<Guid> locationIds, Department? parent = null)
    {
        return Result.Success(new Department(name, identifier, path, depth, isActive, locationIds, parent));
    }
    
    public UnitResult<Failure> UpdateLocations(IEnumerable<Guid> locationIds)
    {
        _locations = locationIds.Select(locationId => new DepartmentLocation(this.Id, locationId)).ToList();
        UpdatedAt = DateTime.UtcNow;
        return UnitResult.Success<Failure>();
    }
}
