using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Locations;

public record class LocationName
{
    private const int MAX_LENGTH = 120;
    private const int MIN_LENGTH = 3;

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

        if (name.Length > MAX_LENGTH)
        {
            return Result.Failure<LocationName>($"Location name cannot exceed {MAX_LENGTH} characters.");
        }

        if (name.Length < MIN_LENGTH)
        {
            return Result.Failure<LocationName>($"Location name must be at least {MIN_LENGTH} characters.");
        }

        return Result.Success(new LocationName(name));
    }

}
