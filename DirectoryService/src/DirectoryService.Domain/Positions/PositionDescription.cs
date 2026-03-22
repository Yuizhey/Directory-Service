using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Positions;

public record class PositionDescription
{
    private PositionDescription(string value)
    {
        Value = value;
    }

    public string Value { get; }
    
    public static Result<PositionDescription, Failure> Create(string description)
    {
        var errors = new List<Error>();
        if (string.IsNullOrWhiteSpace(description))
        {
            errors.Add(Error.BadRequest("Position description cannot be empty."));
        }

        if (description.Length > LengthConstants.MAX_LENGTH_1000)
        {
            errors.Add(Error.BadRequest($"Position description cannot exceed {LengthConstants.MAX_LENGTH_1000} characters."));
        }

        if (errors.Any())
        {
            return Result.Failure<PositionDescription, Failure>(errors);
        }

        return Result.Success<PositionDescription, Failure>(new PositionDescription(description));
    }
}
