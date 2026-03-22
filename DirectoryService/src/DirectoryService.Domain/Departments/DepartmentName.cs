using System.Reflection.Metadata;
using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Departments;

public sealed record class DepartmentName
{
    private DepartmentName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<DepartmentName, Failure> Create(string name)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(Error.BadRequest("Department name cannot be empty."));
        }

        if (name.Length > LengthConstants.MAX_LENGTH_150)
        {
            errors.Add(Error.BadRequest($"Department name cannot exceed {LengthConstants.MAX_LENGTH_150} characters."));
        }

        if (name.Length < LengthConstants.MIN_LENGTH_3)
        {
            errors.Add(Error.BadRequest($"Department name must be at least {LengthConstants.MIN_LENGTH_3} characters."));
        }

        if (errors.Any())
        {
            return Result.Failure<DepartmentName, Failure>(errors);
        }

        return Result.Success<DepartmentName, Failure>(new DepartmentName(name));
    }
}
