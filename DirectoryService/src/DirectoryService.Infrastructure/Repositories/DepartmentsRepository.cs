using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Departments;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared.Errors;

namespace DirectoryService.Infrastructure.Repositories;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly ILogger<DepartmentsRepository> _logger;
    private readonly DirectoryServiceDbContext _dbContext;

    public DepartmentsRepository(ILogger<DepartmentsRepository> logger, DirectoryServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Guid, Failure>> Create(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success<Guid, Failure>(department.Id);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException pgEx &&
                pgEx.SqlState == PostgresErrorCodes.UniqueViolation &&
                pgEx.ConstraintName == "ix_departments_identifier")
            {
                _logger.LogWarning(
                    "Попытка создать отдел с уже существующим identifier (DepartmentId={DepartmentId}, Identifier={Identifier})",
                    department.Id,
                    department.Identifier.Value);
                return Result.Failure<Guid, Failure>(Error.Conflict("A department with the same identifier already exists"));
            }

            _logger.LogError(
                ex,
                "Ошибка при сохранении отдела в БД (DepartmentId={DepartmentId})",
                department.Id);
            return Result.Failure<Guid, Failure>(Error.Conflict("An error occurred while saving the department to the database"));
        }
    }

    public async Task<UnitResult<Failure>> DeleteAllLocationsByDepartmentId(Guid departmentId, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.DepartmentLocations.Where(l => l.DepartmentId == departmentId).ExecuteDeleteAsync(cancellationToken);
            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ошибка при удалении всех локаций для отдела (DepartmentId={DepartmentId})",
                departmentId);
            return UnitResult.Failure<Failure>(Error.Conflict("An error occurred while deleting the department's locations from the database"));
        }
    }

    public async Task<Result<Department, Failure>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            if (department == null)
            {
                return Result.Failure<Department, Failure>(Error.NotFound("Department not found"));
            }

            return Result.Success<Department, Failure>(department);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ошибка при получении отдела по идентификатору (DepartmentId={DepartmentId})",
                id);
            return Result.Failure<Department, Failure>(Error.Conflict("An error occurred while retrieving the department from the database"));
        }
    }


    public async Task<Result<List<Department>, Failure>> GetByIds(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        try
        {
            var departments = await _dbContext.Departments.Where(d => ids.Contains(d.Id)).ToListAsync(cancellationToken);
            return Result.Success<List<Department>, Failure>(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ошибка при получении отделов по идентификаторам (DepartmentIds={DepartmentIds})",
                string.Join(", ", ids));
            return Result.Failure<List<Department>, Failure>(Error.Conflict("An error occurred while retrieving the departments from the database"));
        }
    }
}
