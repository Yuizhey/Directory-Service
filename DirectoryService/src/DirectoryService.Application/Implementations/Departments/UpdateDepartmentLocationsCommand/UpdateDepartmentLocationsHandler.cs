using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Abstractions.DatabaseHelpers;
using DirectoryService.Application.Abstractions.Departments;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Application.Extensions;
using DirectoryService.Contracts.Departments.Update;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Application.Implementations.Departments.UpdateDepartmentLocationsCommand;

public record UpdateDepartmentLocationsCommand(Guid Id, UpdateDepartmentLocationsRequest Request) : ICommand;

public sealed class UpdateDepartmentLOcationsCommandValidator : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLOcationsCommandValidator()
    {
        RuleFor(p => p.Request.LocationIds)
            .NotEmpty()
            .WithError(Error.BadRequest("At least one location must be associated with the department."));

        RuleFor(p => p.Request.LocationIds)
            .Must(p => p.Distinct().Count() == p.Count())
            .WithError(Error.BadRequest("Duplicate location IDs are not allowed."));
    }
}

public sealed class UpdateDepartmentLocationsHandler : ICommandHandler<UpdateDepartmentLocationsCommand>
{
    private readonly ILogger<UpdateDepartmentLocationsHandler> _logger;
    private readonly IValidator<UpdateDepartmentLocationsCommand> _validator;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;

    public UpdateDepartmentLocationsHandler(
        ILogger<UpdateDepartmentLocationsHandler> logger,
        IValidator<UpdateDepartmentLocationsCommand> validator,
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager)
    {
        _logger = logger;
        _validator = validator;
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<UnitResult<Failure>> Handle(UpdateDepartmentLocationsCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            var validationResultErrors = validationResult.Errors.ToErrorList();
            return UnitResult.Failure<Failure>(validationResultErrors);
        }

        var transactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
        {
            return UnitResult.Failure<Failure>(transactionResult.Error);
        }

        var transaction = transactionResult.Value;

        var foundLocations = await _locationsRepository.GetByIdsAsync(command.Request.LocationIds, cancellationToken);
        if (foundLocations.IsFailure)
        {
            transaction.Rollback();
            return UnitResult.Failure<Failure>(foundLocations.Error);
        }

        var locationsErrors = new List<Error>();
        foreach (var locationtId in command.Request.LocationIds)
        {
            if (!foundLocations.Value.Any(d => d.Id == locationtId))
            {
                locationsErrors.Add(Error.NotFound($"Location with ID {locationtId} was not found."));
            }
        }

        if (locationsErrors.Any())
        {
            transaction.Rollback();
            return UnitResult.Failure<Failure>(locationsErrors);
        }

        var departmentResult = await _departmentsRepository.GetById(command.Id, cancellationToken);
        if (departmentResult.IsFailure)
        {
            transaction.Rollback();
            return UnitResult.Failure<Failure>(departmentResult.Error);
        }

        var updateResult = departmentResult.Value.UpdateLocations(command.Request.LocationIds);

        var deleteResult = await _departmentsRepository.DeleteAllLocationsByDepartmentId(departmentResult.Value.Id, cancellationToken);
        if (deleteResult.IsFailure)
        {
            transaction.Rollback();
            return UnitResult.Failure<Failure>(deleteResult.Error);
        }

        await _transactionManager.SaveChangesAsync(cancellationToken);
        transaction.Commit();
        return UnitResult.Success<Failure>();
    }
}
