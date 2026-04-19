using System;

namespace DirectoryService.Contracts.Departments.Update;

public sealed record UpdateDepartmentsStructureRequest(Guid newParentDepartmentId);