using System;
using System.ComponentModel;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public class Department
{
    private List<Department> _children = [];
    private List<DepartmentLocation> _locations = [];
    private List<DepartmentPosition> _positions = [];

    private Department(DepartmentName name, DepartmentIdentifier identifier, DepartmentPath path, short depth, bool isActive, IEnumerable<Guid>? locationIds = null, IEnumerable<Guid>? positionIds = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        Path = path;
        Depth = depth;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        _locations = locationIds?.Select(locationId => new DepartmentLocation(this.Id, locationId)).ToList() ?? [];
        _positions = positionIds?.Select(positionId => new DepartmentPosition(this.Id, positionId)).ToList() ?? [];
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

    public static Result<Department> Create(string name, string identifier, string path, short depth, bool isActive, Department? parent = null, IEnumerable<Guid>? locationIds = null, IEnumerable<Guid>? positionIds = null)
    {
        var nameResult = DepartmentName.Create(name);
        if (nameResult.IsFailure)
        {
            return Result.Failure<Department>(nameResult.Error);
        }

        var identifierResult = DepartmentIdentifier.Create(identifier);
        if (identifierResult.IsFailure)
        {
            return Result.Failure<Department>(identifierResult.Error);
        }

        var pathResult = DepartmentPath.Create(path, parent);
        if (pathResult.IsFailure)
        {
            return Result.Failure<Department>(pathResult.Error);
        }

        return new Department(nameResult.Value, identifierResult.Value, pathResult.Value, depth, isActive, locationIds, positionIds);
    }
    
    public static Result<Department> Create(DepartmentName name, DepartmentIdentifier identifier, DepartmentPath path, short depth, bool isActive, IEnumerable<Guid>? locationIds = null, IEnumerable<Guid>? positionIds = null)
    { 
        return Result.Success(new Department(name, identifier, path, depth, isActive, locationIds, positionIds));
    }
}
