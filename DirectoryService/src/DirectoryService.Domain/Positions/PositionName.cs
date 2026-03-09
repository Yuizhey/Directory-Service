using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Positions;

public record class PositionName
{
    private PositionName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PositionName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<PositionName>("Position name cannot be empty.");
        }

        if (name.Length > LengthConstants.MAX_LENGTH_100)
        {
            return Result.Failure<PositionName>($"Position name cannot exceed {LengthConstants.MAX_LENGTH_100} characters.");
        }

        if (name.Length < LengthConstants.MIN_LENGTH_3)
        {
            return Result.Failure<PositionName>($"Position name must be at least {LengthConstants.MIN_LENGTH_3} characters.");
        }

        return Result.Success(new PositionName(name));
    }
}
