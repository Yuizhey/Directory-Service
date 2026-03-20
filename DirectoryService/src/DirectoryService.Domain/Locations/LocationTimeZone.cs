using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Locations;

public record class LocationTimeZone
{
    private LocationTimeZone(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<LocationTimeZone, Failure> Create(string timeZone)
    {
        var errors = new List<Error>();
        if (string.IsNullOrWhiteSpace(timeZone))
        {
            errors.Add(Error.BadRequest("Time zone cannot be empty."));
        }

        if (!Regex.IsMatch(timeZone, RegexConstants.IanaTimeZoneRegex))
        {
            errors.Add(Error.BadRequest("Time zone must be in valid IANA format (e.g., 'America/New_York')."));
        }

        if (timeZone.Length > LengthConstants.MAX_LENGTH_120)
        {
            errors.Add(Error.BadRequest($"Time zone cannot exceed {LengthConstants.MAX_LENGTH_120} characters."));
        }

        if (timeZone.Length < LengthConstants.MIN_LENGTH_5)
        {
            errors.Add(Error.BadRequest($"Time zone must be at least {LengthConstants.MIN_LENGTH_5} characters."));
        }

        if (errors.Any())
        {
            return Result.Failure<LocationTimeZone, Failure>(errors);
        }

        return Result.Success<LocationTimeZone, Failure>(new LocationTimeZone(timeZone));
    }
}
