namespace DirectoryService.Contracts.Locations.Create;

public sealed record class CreateLocationAddressRequest(string country, string city, string street, int houseNumber);