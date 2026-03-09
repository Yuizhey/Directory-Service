using System.Reflection.Metadata;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public sealed record class DepartmentName
{
    private DepartmentName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<DepartmentName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<DepartmentName>("Department name cannot be empty.");
        }

        if (name.Length > LengthConstants.MAX_LENGTH_150)
        {
            return Result.Failure<DepartmentName>($"Department name cannot exceed {LengthConstants.MAX_LENGTH_150} characters.");
        }

        if (name.Length < LengthConstants.MIN_LENGTH_3)
        {
            return Result.Failure<DepartmentName>($"Department name must be at least {LengthConstants.MIN_LENGTH_3} characters.");
        }

        return Result.Success(new DepartmentName(name));
    }
}
