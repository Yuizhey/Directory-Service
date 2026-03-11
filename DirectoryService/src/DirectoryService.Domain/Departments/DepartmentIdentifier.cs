using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public record class DepartmentIdentifier
{
    private DepartmentIdentifier(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<DepartmentIdentifier> Create(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Failure<DepartmentIdentifier>("Department identifier cannot be empty.");
        }

        if(!Regex.IsMatch(identifier, RegexConstants.LATIN_REGEX))
        {
            return Result.Failure<DepartmentIdentifier>("Department identifier must contain only Latin letters (a-z, A-Z).");
        }

        if (identifier.Length > LengthConstants.MAX_LENGTH_150)
        {
            return Result.Failure<DepartmentIdentifier>($"Department identifier cannot exceed {LengthConstants.MAX_LENGTH_150} characters.");
        }

        if (identifier.Length < LengthConstants.MIN_LENGTH_3)
        {
            return Result.Failure<DepartmentIdentifier>($"Department identifier must be at least {LengthConstants.MIN_LENGTH_3} characters.");
        }

        return Result.Success(new DepartmentIdentifier(identifier));
    }
}
