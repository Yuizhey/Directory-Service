using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Departments;

public record class DepartmentIdentifier
{
    private DepartmentIdentifier(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<DepartmentIdentifier, Failure> Create(string identifier)
    {
        var errors = new List<Error>();
        if (string.IsNullOrWhiteSpace(identifier))
        {
            errors.Add(Error.BadRequest("Department identifier cannot be empty."));
        }

        if (!Regex.IsMatch(identifier, RegexConstants.LATIN_REGEX))
        {
            errors.Add(Error.BadRequest("Department identifier must contain only Latin letters (a-z, A-Z)."));
        }

        if (identifier.Length > LengthConstants.MAX_LENGTH_150)
        {
            errors.Add(Error.BadRequest($"Department identifier cannot exceed {LengthConstants.MAX_LENGTH_150} characters."));
        }

        if (identifier.Length < LengthConstants.MIN_LENGTH_3)
        {
            errors.Add(Error.BadRequest($"Department identifier must be at least {LengthConstants.MIN_LENGTH_3} characters."));
        }

        if (errors.Any())
        {
            return Result.Failure<DepartmentIdentifier, Failure>(errors);
        }

        return Result.Success<DepartmentIdentifier, Failure>(new DepartmentIdentifier(identifier));
    }
}
