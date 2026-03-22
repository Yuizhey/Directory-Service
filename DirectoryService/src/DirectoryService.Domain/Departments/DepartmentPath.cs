using CSharpFunctionalExtensions;
using Shared.Errors;

namespace DirectoryService.Domain.Departments;

public record class DepartmentPath
{
    private const string Separator = ".";
    
    private DepartmentPath(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<DepartmentPath, Failure> Create(string path, Department? parent = null)
    {
        var errors = new List<Error>();
        if (string.IsNullOrWhiteSpace(path))
        {
            errors.Add(Error.BadRequest("Department path cannot be empty."));
        }

        if (path.Length > LengthConstants.MAX_LENGTH_50)
        {
            errors.Add(Error.BadRequest($"Department path cannot exceed {LengthConstants.MAX_LENGTH_50} characters."));
        }

        if (path.Length < LengthConstants.MIN_LENGTH_2)
        {
            errors.Add(Error.BadRequest($"Department path must be at least {LengthConstants.MIN_LENGTH_2} characters."));
        }

        if (errors.Any())
        {
            return Result.Failure<DepartmentPath, Failure>(errors);
        }

        if (parent is null)
        {
            return Result.Success<DepartmentPath, Failure>(new DepartmentPath(path));
        }

        return Result.Success<DepartmentPath, Failure>(new DepartmentPath($"{parent.Path.Value}{Separator}{path}"));
    }
}
