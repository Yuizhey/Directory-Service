using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Abstractions.Departments;
using DirectoryService.Application.Abstractions.Positions;
using DirectoryService.Application.Extensions;
using DirectoryService.Contracts.Positions.Create;
using DirectoryService.Domain.Positions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Application.Implementations.Positions.CreatePositionCommand;

public sealed record CreatePositionCommand(CreatePositionRequest request) : ICommand;

public sealed class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(p => p.request.name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(p => p.request.description)
            .MustBeValueObject(PositionDescription.Create);

        RuleFor(p => p.request.departmentIds)
            .NotEmpty()
            .WithError(Error.BadRequest("At least one department must be associated with the position."));

        RuleFor(p => p.request.departmentIds)
            .Must(p => p.Distinct().Count() == p.Count())
            .WithError(Error.BadRequest("Duplicate department IDs are not allowed."));
    }
}

public sealed class CreatePositionHandler : ICommandHandler<Guid, CreatePositionCommand>
{
    private readonly ILogger<CreatePositionHandler> _logger;
    private readonly IPositionsRepository _positionsRepository;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly IValidator<CreatePositionCommand> _validator;

    public CreatePositionHandler(
        IValidator<CreatePositionCommand> validator, 
        ILogger<CreatePositionHandler> logger, 
        IPositionsRepository positionsRepository,
        IDepartmentsRepository departmentsRepository)
    {
        _logger = logger;
        _positionsRepository = positionsRepository;
        _validator = validator;
        _departmentsRepository = departmentsRepository;
    }

    public async Task<Result<Guid, Failure>> Handle(CreatePositionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var validationResultErrors = validationResult.Errors.ToErrorList();
            return Result.Failure<Guid, Failure>(validationResultErrors);
        }

        var foundDepartments = await _departmentsRepository.GetByIds(command.request.departmentIds, cancellationToken);
        if (foundDepartments.IsFailure)
        {
            _logger.LogWarning("Отклонён запрос на создание должности");
            return Result.Failure<Guid, Failure>(foundDepartments.Error);
        }

        var departmentErrors = new List<Error>();
        foreach (var department in foundDepartments.Value)
        {
            if (!department.IsActive)
            {
                departmentErrors.Add(Error.BadRequest($"Department with ID {department.Id} is inactive and cannot be associated with a position."));
                continue;
            }
        }

        foreach (var departmentId in command.request.departmentIds)
        {
            if (!foundDepartments.Value.Any(d => d.Id == departmentId))
            {
                departmentErrors.Add(Error.NotFound($"Department with ID {departmentId} was not found."));
            }
        }

        if (departmentErrors.Any())
        {
            return Result.Failure<Guid, Failure>(departmentErrors);
        }

        var positionName = PositionName.Create(command.request.name);
        if (positionName.IsFailure)
        {
            departmentErrors.AddRange(positionName.Error.Errors);
        }

        var positionDescription = PositionDescription.Create(command.request.description);
        if (positionDescription.IsFailure)
        {
            departmentErrors.AddRange(positionDescription.Error.Errors);
        }

        if (departmentErrors.Any())
        {
            _logger.LogWarning(
                "Отклонён запрос на создание должность: ошибки валидации: {Errors}",
                string.Join("; ", departmentErrors.Select(e => $"{e.Code}: {e.Message}")));
            return Result.Failure<Guid, Failure>(departmentErrors);
        }

        var result = Position.Create(positionName.Value, positionDescription.Value, isActive: true, command.request.departmentIds);
        var createResult = await _positionsRepository.Create(result.Value, cancellationToken);
        if (createResult.IsFailure)
        {
            _logger.LogWarning(
                "Создание должность не завершено: {Errors}",
                string.Join("; ", createResult.Error.Select(e => $"{e.Code}: {e.Message}")));
            return Result.Failure<Guid, Failure>(createResult.Error);
        }

        _logger.LogInformation(
            "Создана должность: {PositionId}",
            createResult.Value);
        return Result.Success<Guid, Failure>(createResult.Value);
    }
}