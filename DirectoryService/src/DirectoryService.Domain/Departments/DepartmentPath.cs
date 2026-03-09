using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public record class DepartmentPath
{
    private const string Separator = ".";
    
    private DepartmentPath(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<DepartmentPath> Create(string path, Department? parent = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<DepartmentPath>("Department path cannot be empty.");
        }

        if (path.Length > LengthConstants.MAX_LENGTH_50)
        {
            return Result.Failure<DepartmentPath>($"Department path cannot exceed {LengthConstants.MAX_LENGTH_50} characters.");
        }

        if (path.Length < LengthConstants.MIN_LENGTH_2)
        {
            return Result.Failure<DepartmentPath>($"Department path must be at least {LengthConstants.MIN_LENGTH_2} characters.");
        }

        if (parent is null)
        {
            return Result.Success(new DepartmentPath(path));
        }

        return Result.Success(new DepartmentPath($"{parent.Path.Value}{Separator}{path}"));
    }
}
