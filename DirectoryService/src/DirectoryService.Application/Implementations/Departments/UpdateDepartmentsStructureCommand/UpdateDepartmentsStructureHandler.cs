using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Abstractions.Departments;
using DirectoryService.Contracts.Departments.Update;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Application.Implementations.Departments.UpdateDepartmentsStructureCommand;

public record UpdateDepartmentsStructureCommand(Guid departmentId, UpdateDepartmentsStructureRequest Request) : ICommand;

public sealed class UpdateDepartmentsStructureHandler : ICommandHandler<UpdateDepartmentsStructureCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILogger<UpdateDepartmentsStructureHandler> _logger;

    public UpdateDepartmentsStructureHandler(IDepartmentsRepository departmentsRepository, ILogger<UpdateDepartmentsStructureHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _logger = logger;
    }

    public Task<UnitResult<Failure>> Handle(UpdateDepartmentsStructureCommand command, CancellationToken cancellationToken) => throw new NotImplementedException();
}
