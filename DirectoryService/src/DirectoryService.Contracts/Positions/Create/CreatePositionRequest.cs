namespace DirectoryService.Contracts.Positions.Create;

public sealed record class CreatePositionRequest(string name, string? description, Guid[] departmentIds);