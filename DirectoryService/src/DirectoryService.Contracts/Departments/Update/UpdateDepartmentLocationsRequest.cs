namespace DirectoryService.Contracts.Departments.Update;

public record class UpdateDepartmentLocationsRequest(IEnumerable<Guid> LocationIds);