using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Locations;

public record class LocationTimeZone
{
    private const int MAX_LENGTH = 120;
    private const int MIN_LENGTH = 5;
    public const string IanaTimeZoneRegex = @"^[A-Za-z]+\/[A-Za-z_]+$";
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

        if (!Regex.IsMatch(timeZone, IanaTimeZoneRegex))
        {
            return Result.Failure<LocationTimeZone>("Position time zone must be in valid IANA format (e.g., 'America/New_York').");
        }

        if (timeZone.Length > MAX_LENGTH)
        {
            return Result.Failure<LocationTimeZone>($"Position name cannot exceed {MAX_LENGTH} characters.");
        }

        if (timeZone.Length < MIN_LENGTH)
        {
            return Result.Failure<LocationTimeZone>($"Position name must be at least {MIN_LENGTH} characters.");
        }

        return Result.Success(new LocationTimeZone(timeZone));
    }
}
