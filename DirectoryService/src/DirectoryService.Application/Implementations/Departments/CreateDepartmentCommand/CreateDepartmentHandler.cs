using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Abstractions.Departments;
using DirectoryService.Application.Abstractions.Locations;
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

        RuleFor(p => p.request.locationIds)
            .NotEmpty()
            .WithError(Error.BadRequest("At least one location must be associated with the department."));

        RuleFor(p => p.request.locationIds)
            .Must(p => p.Distinct().Count() == p.Count())
            .WithError(Error.BadRequest("Duplicate location IDs are not allowed."));
    }
}

public sealed class CreateDepartmentHandler : ICommandHandler<Guid, CreateDepartmentCommand>
{
    private readonly ILogger<CreateDepartmentHandler> _logger;
    private readonly IValidator<CreateDepartmentCommand> _validator;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;

    public CreateDepartmentHandler(
        ILogger<CreateDepartmentHandler> logger, 
        IValidator<CreateDepartmentCommand> validator, 
        IDepartmentsRepository departmentsRepository, 
        ILocationsRepository locationsRepository)
    {
        _logger = logger;
        _validator = validator;
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
    }

    public async Task<Result<Guid, Failure>> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var validationResultErrors = validationResult.Errors.Select(e => e.CustomState as Failure).SelectMany(f => f!.Errors).ToList();
            return Result.Failure<Guid, Failure>(validationResultErrors);
        }

        var foundLocations = await _locationsRepository.GetByIdsAsync(command.request.locationIds, cancellationToken);
        if (foundLocations.IsFailure)
        {
            return Result.Failure<Guid, Failure>(foundLocations.Error);
        }

        var locationsErrors = new List<Error>();
        foreach (var locationtId in command.request.locationIds)
        {
            if (!foundLocations.Value.Any(d => d.Id == locationtId))
            {
                locationsErrors.Add(Error.NotFound($"Location with ID {locationtId} was not found."));
            }
        }

        if (locationsErrors.Any())
        {
            return Result.Failure<Guid, Failure>(locationsErrors);
        }

        var name = DepartmentName.Create(command.request.name);
        if (name.IsFailure)
        {
            locationsErrors.AddRange(name.Error);
        }

        var identifier = DepartmentIdentifier.Create(command.request.identifier);
        if (identifier.IsFailure)
        {
            locationsErrors.AddRange(identifier.Error);
        }

        if (command.request.parentId == null)
        {
            var path = DepartmentPath.Create(command.request.identifier);
            if (path.IsFailure)
            {
                locationsErrors.AddRange(path.Error);
            }
            
            if (locationsErrors.Any())
            {
                _logger.LogWarning(
                    "Отклонён запрос на создание отдела: ошибки валидации: {Errors}",
                    string.Join("; ", locationsErrors.Select(e => $"{e.Code}: {e.Message}")));
                return Result.Failure<Guid, Failure>(locationsErrors);
            }

            var department = Department.Create(
                name.Value,
                identifier.Value,
                path.Value,
                depth: 0,
                isActive: true,
                locationIds: command.request.locationIds);

            var createResult = await _departmentsRepository.Create(department.Value, cancellationToken);
            if (createResult.IsFailure)
            {
                _logger.LogWarning(
                    "Создание отдела не завершено: {Errors}",
                    string.Join("; ", createResult.Error.Select(e => $"{e.Code}: {e.Message}")));
                return Result.Failure<Guid, Failure>(createResult.Error);
            }
            
            _logger.LogInformation(
                "Создана должность: {PositionId}",
                createResult.Value);

            return Result.Success<Guid, Failure>(createResult.Value);
        }
        else
        {
            var foundParentDepartment = await _departmentsRepository.GetById(command.request.parentId.Value, cancellationToken);
            if (foundParentDepartment.IsFailure)
            {
                return Result.Failure<Guid, Failure>(foundParentDepartment.Error);
            }

            var path = DepartmentPath.Create(command.request.identifier, foundParentDepartment.Value);
            if (path.IsFailure)
            {
                locationsErrors.AddRange(path.Error);
            }
            
            if (locationsErrors.Any())
            {
                _logger.LogWarning(
                    "Отклонён запрос на создание отдела: ошибки валидации: {Errors}",
                    string.Join("; ", locationsErrors.Select(e => $"{e.Code}: {e.Message}")));
                return Result.Failure<Guid, Failure>(locationsErrors);
            }

            var department = Department.Create(
                name.Value,
                identifier.Value,
                path.Value,
                depth: (short)(foundParentDepartment.Value.Depth + 1),
                isActive: true,
                locationIds: command.request.locationIds,
                foundParentDepartment.Value);

            var createResult = await _departmentsRepository.Create(department.Value, cancellationToken);
            if (createResult.IsFailure)
            {
                _logger.LogWarning(
                    "Создание отдела не завершено: {Errors}",
                    string.Join("; ", createResult.Error.Select(e => $"{e.Code}: {e.Message}")));
                return Result.Failure<Guid, Failure>(createResult.Error);
            }
            
            _logger.LogInformation(
                "Создана должность: {PositionId}",
                createResult.Value);

            return Result.Success<Guid, Failure>(createResult.Value);
        }
    }
}
