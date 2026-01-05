using System;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public class Department
{
    private List<Department> _children = [];

    private Department(DepartmentName name, DepartmentIdentifier identifier, DepartmentPath path, short depth, bool isActive)
    {
        Id = Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        Path = path;
        Depth = depth;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
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

    public static Result<Department> Create(string name, string identifier, string path, short depth, bool isActive)
    {
        var nameResult = DepartmentName.Create(name);
        if(nameResult.IsFailure)
        {
            return Result.Failure<Department>(nameResult.Error);
        }

        var identifierResult = DepartmentIdentifier.Create(identifier);
        if(identifierResult.IsFailure)
        {
            return Result.Failure<Department>(identifierResult.Error);
        }

        var pathResult = DepartmentPath.Create(path, depth == 1 ? string.Empty : path);
        if(pathResult.IsFailure)
        {
            return Result.Failure<Department>(pathResult.Error);
        }

        return new Department(nameResult.Value, identifierResult.Value, pathResult.Value, depth, isActive);
    }
}
