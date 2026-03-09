using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Locations;

public record class LocationName
{
    private LocationName(string value)
    {
        Value = value;
    }

    public string Value { get; set; }
    
    public static Result<LocationName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<LocationName>("Location name cannot be empty.");
        }

        if (name.Length > LengthConstants.MAX_LENGTH_120)
        {
            return Result.Failure<LocationName>($"Location name cannot exceed {LengthConstants.MAX_LENGTH_120} characters.");
        }

        if (name.Length < LengthConstants.MIN_LENGTH_3)
        {
            return Result.Failure<LocationName>($"Location name must be at least {LengthConstants.MIN_LENGTH_3} characters.");
        }

        return Result.Success(new LocationName(name));
    }

}
