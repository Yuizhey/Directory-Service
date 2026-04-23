using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Abstractions.DatabaseHelpers;
using DirectoryService.Application.Abstractions.Departments;
using DirectoryService.Contracts.Departments.Update;
using DirectoryService.Contracts.Departments.Update.Response;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Application.Implementations.Departments.UpdateDepartmentsStructureCommand;

public record UpdateDepartmentsStructureCommand(Guid departmentId, UpdateDepartmentsStructureRequest Request) : ICommand;

public sealed class UpdateDepartmentsStructureHandler : ICommandHandler<UpdateDepartmentsStructureCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<UpdateDepartmentsStructureHandler> _logger;

    public UpdateDepartmentsStructureHandler(
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        ILogger<UpdateDepartmentsStructureHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<UnitResult<Failure>> Handle(UpdateDepartmentsStructureCommand command, CancellationToken cancellationToken)
    {
        var transactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
        {
            return UnitResult.Failure<Failure>(transactionResult.Error);
        }

        using var transaction = transactionResult.Value;

        var departmentIds = command.Request.ParentId is null
            ? new[] { command.departmentId }
            : new[] { command.departmentId, command.Request.ParentId.Value }.Distinct().ToArray();

        var departmentsResult = await _departmentsRepository.GetByIdsForUpdate(departmentIds, cancellationToken);
        if (departmentsResult.IsFailure)
        {
            transaction.Rollback();
            return UnitResult.Failure<Failure>(departmentsResult.Error);
        }

        var department = departmentsResult.Value.FirstOrDefault(d => d.Id == command.departmentId);
        if (department is null)
        {
            transaction.Rollback();
            return UnitResult.Failure<Failure>(Error.NotFound($"Department with ID {command.departmentId} was not found."));
        }

        if (!department.IsActive)
        {
            transaction.Rollback();
            return UnitResult.Failure<Failure>(Error.BadRequest($"Department with ID {command.departmentId} is inactive and cannot be moved."));
        }

        DepartmentStructureInfo? parentDepartment = null;
        if (command.Request.ParentId is not null)
        {
            if (command.Request.ParentId == command.departmentId)
            {
                transaction.Rollback();
                return UnitResult.Failure<Failure>(Error.BadRequest("Department cannot be moved under itself."));
            }

            parentDepartment = departmentsResult.Value.FirstOrDefault(d => d.Id == command.Request.ParentId.Value);
            if (parentDepartment is null)
            {
                transaction.Rollback();
                return UnitResult.Failure<Failure>(Error.NotFound($"Parent department with ID {command.Request.ParentId} was not found."));
            }

            if (!parentDepartment.IsActive)
            {
                transaction.Rollback();
                return UnitResult.Failure<Failure>(Error.BadRequest($"Parent department with ID {command.Request.ParentId} is inactive."));
            }

            if (parentDepartment.Path.StartsWith($"{department.Path}.", StringComparison.Ordinal))
            {
                transaction.Rollback();
                return UnitResult.Failure<Failure>(Error.BadRequest("Department cannot be moved under one of its child departments."));
            }
        }

        var moveResult = await _departmentsRepository.MoveDepartment(command.departmentId, command.Request.ParentId, cancellationToken);
        if (moveResult.IsFailure)
        {
            transaction.Rollback();
            return UnitResult.Failure<Failure>(moveResult.Error);
        }

        var commitResult = transaction.Commit();
        if (commitResult.IsFailure)
        {
            return UnitResult.Failure<Failure>(commitResult.Error);
        }

        _logger.LogInformation(
            "Подразделение перенесено: DepartmentId={DepartmentId}, ParentId={ParentId}",
            command.departmentId,
            command.Request.ParentId);

        return UnitResult.Success<Failure>();
    }
}
