using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Locations;

public record class LocationAddress
{
    private const int MAX_LENGTH = 50;

    public LocationAddress(string country, string city, string street, int houseNumber)
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

    public static Result<LocationAddress> Create(string country, string city, string street, int houseNumber)
    {
        if (!IsValidStringPart(country))
        {
            return Result.Failure<LocationAddress>($"Country cannot be empty or exceed {MAX_LENGTH} characters.");
        }

        if (!IsValidStringPart(city))
        {
            return Result.Failure<LocationAddress>($"City cannot be empty or exceed {MAX_LENGTH} characters.");
        }

        if (!IsValidStringPart(street))
        {
            return Result.Failure<LocationAddress>($"Street cannot be empty or exceed {MAX_LENGTH} characters.");
        }

        if (houseNumber <= 0)
        {
            return Result.Failure<LocationAddress>("House number must be a positive integer.");
        }

        return Result.Success(new LocationAddress(country, city, street, houseNumber));
    }

    private static bool IsValidStringPart(string part)
    {
        return !string.IsNullOrWhiteSpace(part) && part.Length <= MAX_LENGTH;
    }
}
