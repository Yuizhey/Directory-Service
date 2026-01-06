using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Positions;

public record class PositionDescription
{
    private const int MAX_LENGTH = 1000;

    private PositionDescription(string value)
    {
        Value = value;
    }

    public string Value { get; }
    
    public static Result<PositionDescription> Create(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Result.Failure<PositionDescription>("Position description cannot be empty.");
        }

        if (description.Length > MAX_LENGTH)
        {
            return Result.Failure<PositionDescription>($"Position description cannot exceed {MAX_LENGTH} characters.");
        }

        return Result.Success(new PositionDescription(description));
    }
}
