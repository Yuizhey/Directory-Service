using System;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using Shared.Errors;

namespace DirectoryService.Domain.Locations;

public class Location
{
    private List<DepartmentLocation> _departments = [];

    /// <summary>
    /// Private constructor for EF Core. Do not use it directly to create Location instances.
    /// </summary>
    private Location() { }

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
    
    public static Result<Location, Failure> Create(LocationName name, LocationAddress address, LocationTimeZone timeZone, bool isActive = true)
    {
        return Result.Success<Location, Failure>(new Location(name, address, timeZone, isActive));
    }
}
