using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public record class DepartmentIdentifier
{
    private const int MAX_LENGTH = 150;
    private const int MIN_LENGTH = 3;
    private const string LATIN_REGEX = @"^[a-zA-Z]+$";

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

        if(!Regex.IsMatch(identifier, LATIN_REGEX))
        {
            return Result.Failure<DepartmentIdentifier>("Department identifier must contain only Latin letters (a-z, A-Z).");
        }

        if (identifier.Length > MAX_LENGTH)
        {
            return Result.Failure<DepartmentIdentifier>($"Department identifier cannot exceed {MAX_LENGTH} characters.");
        }

        if (identifier.Length < MIN_LENGTH)
        {
            return Result.Failure<DepartmentIdentifier>($"Department identifier must be at least {MIN_LENGTH} characters.");
        }

        return Result.Success(new DepartmentIdentifier(identifier));
    }
}
