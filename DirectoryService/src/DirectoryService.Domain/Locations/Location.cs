using System;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Domain.Locations;

public class Location
{
    private List<DepartmentLocation> _departments = [];

    private Location(LocationName name, LocationAddress address, LocationTimeZone timeZone, bool isActive)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        TimeZone = timeZone;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public LocationName Name { get; private set; }

    public LocationAddress Address { get; private set; }

    public LocationTimeZone TimeZone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> Departments => _departments.AsReadOnly();

    public static Result<Location> Create(string name, string country, string city, string street, int houseNumber, string timeZone, bool isActive)
    {
        var nameResult = LocationName.Create(name);
        if (nameResult.IsFailure)
        {
            return Result.Failure<Location>(nameResult.Error);
        }

        var addressResult = LocationAddress.Create(country, city, street, houseNumber);
        if (addressResult.IsFailure)
        {
            return Result.Failure<Location>(addressResult.Error);
        }

        var timeZoneResult = LocationTimeZone.Create(timeZone);
        if (timeZoneResult.IsFailure)
        {
            return Result.Failure<Location>(timeZoneResult.Error);
        }

        return Result.Success(new Location(nameResult.Value, addressResult.Value, timeZoneResult.Value, isActive));
    }
    
    public static Result<Location> Create(LocationName name, LocationAddress address, LocationTimeZone timeZone, bool isActive)
    {
        return Result.Success(new Location(name, address, timeZone, isActive));
    }
}
