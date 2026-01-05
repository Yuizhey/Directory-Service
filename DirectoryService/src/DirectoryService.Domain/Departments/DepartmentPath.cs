using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Departments;

public record class DepartmentPath
{
    private const int MAX_LENGTH = 50;
    private const int MIN_LENGTH = 2;
    
    private DepartmentPath(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<DepartmentPath> Create(string path, string parentPath)
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

        return Result.Success(new DepartmentPath($"{parentPath}.{path}"));
    }
}
