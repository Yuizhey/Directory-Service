namespace DirectoryService.Contracts.Departments.Create;

public sealed record class CreateDepartmentRequest(string name, string identifier, Guid? parentId, Guid[] locationIds);