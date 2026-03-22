using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Locations;

public record class LocationName
{
    private LocationName(string value)
    {
        Value = value;
    }

    public string Value { get; set; }
    
    public static Result<LocationName, Failure> Create(string name)
    {
        var errors = new List<Error>();
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(Error.BadRequest("Location name cannot be empty."));
        }

        if (name.Length > LengthConstants.MAX_LENGTH_120)
        {
            errors.Add(Error.BadRequest($"Location name cannot exceed {LengthConstants.MAX_LENGTH_120} characters."));
        }

        if (name.Length < LengthConstants.MIN_LENGTH_3)
        {
            errors.Add(Error.BadRequest($"Location name must be at least {LengthConstants.MIN_LENGTH_3} characters."));
        }

        if (errors.Any())
        {
            return Result.Failure<LocationName, Failure>(errors);
        }

        return Result.Success<LocationName, Failure>(new LocationName(name));
    }

}
