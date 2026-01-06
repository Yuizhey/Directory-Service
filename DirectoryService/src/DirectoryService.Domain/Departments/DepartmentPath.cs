using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public record class DepartmentPath
{
    private const int MAX_LENGTH = 50;
    private const int MIN_LENGTH = 2;
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

        if (path.Length > MAX_LENGTH)
        {
            return Result.Failure<DepartmentPath>($"Department path cannot exceed {MAX_LENGTH} characters.");
        }

        if (path.Length < MIN_LENGTH)
        {
            return Result.Failure<DepartmentPath>($"Department path must be at least {MIN_LENGTH} characters.");
        }

        if (parent is null)
        {
            return Result.Success(new DepartmentPath(path));
        }

        return Result.Success(new DepartmentPath($"{parent.Path.Value}{Separator}{path}"));
    }
}
