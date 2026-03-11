using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Positions;

public record class PositionDescription
{
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

        if (description.Length > LengthConstants.MAX_LENGTH_1000)
        {
            return Result.Failure<PositionDescription>($"Position description cannot exceed {LengthConstants.MAX_LENGTH_1000} characters.");
        }

        return Result.Success(new PositionDescription(description));
    }
}
