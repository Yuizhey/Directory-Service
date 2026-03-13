namespace DirectoryService.Contracts.Locations.Create;

public sealed record class CreateLocationRequest(string name, CreateLocationAddressRequest address, string timezone);