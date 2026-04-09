using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Abstractions.Departments;
using DirectoryService.Application.Extensions;
using DirectoryService.Contracts.Departments.Create;
using DirectoryService.Domain.Departments;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Application.Implementations.Departments.CreateDepartmentCommand;

public sealed record CreateDepartmentCommand(CreateDepartmentRequest request) : ICommand;

public sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(d => d.request.name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(d => d.request.identifier)
            .MustBeValueObject(DepartmentIdentifier.Create);
    }
}

public sealed class CreateDepartmentHandler : ICommandHandler<Guid, CreateDepartmentCommand>
{
    private readonly ILogger<CreateDepartmentHandler> _logger;
    private readonly IValidator<CreateDepartmentCommand> _validator;
    private readonly IDepartmentsRepository _departmentsRepository;

    public CreateDepartmentHandler(ILogger<CreateDepartmentHandler> logger, IValidator<CreateDepartmentCommand> validator, IDepartmentsRepository departmentsRepository)
    {
        _logger = logger;
        _validator = validator;
        _departmentsRepository = departmentsRepository;
    }

    public Task<Result<Guid, Failure>> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
