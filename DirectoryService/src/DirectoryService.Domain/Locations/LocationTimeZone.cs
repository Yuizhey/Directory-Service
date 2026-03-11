using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Locations;

public record class LocationTimeZone
{
    private LocationTimeZone(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<LocationTimeZone> Create(string timeZone)
    {
        if (string.IsNullOrWhiteSpace(timeZone))
        {
            return Result.Failure<LocationTimeZone>("Position name cannot be empty.");
        }

        if (!Regex.IsMatch(timeZone, RegexConstants.IanaTimeZoneRegex))
        {
            return Result.Failure<LocationTimeZone>("Position time zone must be in valid IANA format (e.g., 'America/New_York').");
        }

        if (timeZone.Length > LengthConstants.MAX_LENGTH_120)
        {
            return Result.Failure<LocationTimeZone>($"Position name cannot exceed {LengthConstants.MAX_LENGTH_120} characters.");
        }

        if (timeZone.Length < LengthConstants.MIN_LENGTH_5)
        {
            return Result.Failure<LocationTimeZone>($"Position name must be at least {LengthConstants.MIN_LENGTH_5} characters.");
        }

        return Result.Success(new LocationTimeZone(timeZone));
    }
}
