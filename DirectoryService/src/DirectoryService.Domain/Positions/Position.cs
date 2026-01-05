using System;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Positions;

public class Position
{
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
    
    public static Result<Position> Create(string name, string description, bool isActive)
    {
        var nameResult = PositionName.Create(name);
        if (nameResult.IsFailure)
        {
            return Result.Failure<Position>(nameResult.Error);
        }

        var descriptionResult = PositionDescription.Create(description);
        if (descriptionResult.IsFailure)
        {
            return Result.Failure<Position>(descriptionResult.Error);
        }

        return Result.Success(new Position(nameResult.Value, descriptionResult.Value, isActive));
    }
}
