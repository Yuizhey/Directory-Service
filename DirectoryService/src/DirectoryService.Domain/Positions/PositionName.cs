using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Positions;

public record class PositionName
{
    private PositionName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PositionName, Failure> Create(string name)
    {
        var errors = new List<Error>();
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(Error.BadRequest("Position name cannot be empty."));
        }

        if (name.Length > LengthConstants.MAX_LENGTH_100)
        {
            errors.Add(Error.BadRequest($"Position name cannot exceed {LengthConstants.MAX_LENGTH_100} characters."));
        }

        if (name.Length < LengthConstants.MIN_LENGTH_3)
        {
            errors.Add(Error.BadRequest($"Position name must be at least {LengthConstants.MIN_LENGTH_3} characters."));
        }

        if (errors.Any())
        {
            return Result.Failure<PositionName, Failure>(errors);
        }

        return Result.Success<PositionName, Failure>(new PositionName(name));
    }
}
