using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Locations;

public record class LocationAddress
{

    private LocationAddress(string country, string city, string street, int houseNumber)
    {
        Country = country;
        City = city;
        Street = street;
        HouseNumber = houseNumber;
    }

    public string Country { get; }

    public string City { get; }

    public string Street { get; }

    public int HouseNumber { get; }

    public static Result<LocationAddress, Failure> Create(string country, string city, string street, int houseNumber)
    {
        var errors = new List<Error>();

        if (!IsValidStringPart(country))
        {
            errors.Add(Error.BadRequest($"Country cannot be empty or exceed {LengthConstants.MAX_LENGTH_50} characters."));
        }

        if (!IsValidStringPart(city))
        {
            errors.Add(Error.BadRequest($"City cannot be empty or exceed {LengthConstants.MAX_LENGTH_50} characters."));
        }

        if (!IsValidStringPart(street))
        {
            errors.Add(Error.BadRequest($"Street cannot be empty or exceed {LengthConstants.MAX_LENGTH_50} characters."));
        }

        if (houseNumber <= 0)
        {
            errors.Add(Error.BadRequest("House number must be a positive integer."));
        }

        if (errors.Any())
        {
            return Result.Failure<LocationAddress, Failure>(errors);
        }

        return Result.Success<LocationAddress, Failure>(new LocationAddress(country, city, street, houseNumber));
    }

    private static bool IsValidStringPart(string part)
    {
        return !string.IsNullOrWhiteSpace(part) && part.Length <= LengthConstants.MAX_LENGTH_50;
    }
}
