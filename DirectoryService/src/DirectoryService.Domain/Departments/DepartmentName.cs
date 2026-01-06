using System.Reflection.Metadata;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public sealed record class DepartmentName
{
    private const int MAX_LENGTH = 150;
    private const int MIN_LENGTH = 3;
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

        if (name.Length > MAX_LENGTH)
        {
            return Result.Failure<DepartmentName>($"Department name cannot exceed {MAX_LENGTH} characters.");
        }

        if (name.Length < MIN_LENGTH)
        {
            return Result.Failure<DepartmentName>($"Department name must be at least {MIN_LENGTH} characters.");
        }

        return Result.Success(new DepartmentName(name));
    }
}
