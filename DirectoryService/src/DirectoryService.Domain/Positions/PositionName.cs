using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Positions;

public record class PositionName
{
    private const int MAX_LENGTH = 100;
    private const int MIN_LENGTH = 3;
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

        if (name.Length > MAX_LENGTH)
        {
            return Result.Failure<PositionName>($"Position name cannot exceed {MAX_LENGTH} characters.");
        }

        if (name.Length < MIN_LENGTH)
        {
            return Result.Failure<PositionName>($"Position name must be at least {MIN_LENGTH} characters.");
        }

        return Result.Success(new PositionName(name));
    }
}
