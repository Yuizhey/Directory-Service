using System;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using Shared.Errors;

namespace DirectoryService.Domain.Positions;

public class Position
{
    private List<DepartmentPosition> _departments = [];

    /// <summary>
    /// Private constructor for EF Core. Do not use it directly to create Position instances.
    /// </summary>
    private Position() { }
    
    private Position(PositionName name, PositionDescription description, bool isActive)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public PositionName Name { get; private set; }

    public PositionDescription Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> Departments => _departments.AsReadOnly();
    
    public static Result<Position, Failure> Create(PositionName name, PositionDescription description, bool isActive)
    {
        return Result.Success<Position, Failure>(new Position(name, description, isActive));
    }
}
