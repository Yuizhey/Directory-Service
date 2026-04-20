using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Departments;
using DirectoryService.Contracts.Departments.Update.Response;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
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

    public async Task<Result<List<DepartmentStructureInfo>, Failure>> GetByIdsForUpdate(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var departmentIds = ids.Distinct().ToArray();
        try
        {
            var idsParam = new NpgsqlParameter<Guid[]>("ids", departmentIds)
            {
                NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Uuid,
            };

            var departments = await _dbContext.Database
                .SqlQueryRaw<DepartmentStructureInfo>(
                    """
                    SELECT
                        id AS "Id",
                        identifier AS "Identifier",
                        path::text AS "Path",
                        depth AS "Depth",
                        is_active AS "IsActive"
                    FROM departments
                    WHERE id = ANY(@ids)
                    ORDER BY id
                    FOR UPDATE
                    """,
                    idsParam)
                .ToListAsync(cancellationToken);

            return Result.Success<List<DepartmentStructureInfo>, Failure>(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ошибка при получении отделов c блокировкой по идентификаторам (DepartmentIds={DepartmentIds})",
                string.Join(", ", departmentIds));
            return Result.Failure<List<DepartmentStructureInfo>, Failure>(Error.Conflict("An error occurred while retrieving the departments from the database"));
        }
    }

    public async Task<UnitResult<Failure>> MoveDepartment(Guid departmentId, Guid? parentId, CancellationToken cancellationToken)
    {
        try
        {
            var departmentIdParameter = new NpgsqlParameter("departmentId", NpgsqlDbType.Uuid)
            {
                Value = departmentId,
            };
            var parentIdParameter = new NpgsqlParameter("parentId", NpgsqlDbType.Uuid)
            {
                Value = parentId.HasValue ? parentId.Value : DBNull.Value,
            };

            var updatedRows = await _dbContext.Database.ExecuteSqlRawAsync(
                """
                WITH current_department AS (
                    SELECT d.id, d.identifier, d.path, d.depth
                    FROM departments d
                    WHERE d.id = @departmentId
                ),
                parent_department AS (
                    SELECT d.id, d.path, d.depth
                    FROM departments d
                    WHERE d.id = @parentId AND @parentId IS NOT NULL
                ),
                locked_subtree AS MATERIALIZED (
                    SELECT d.id
                    FROM departments d
                    JOIN current_department c ON d.path <@ c.path
                    ORDER BY d.path
                    FOR UPDATE OF d
                ),
                    move AS (
                    SELECT
                        c.id AS department_id,
                        @parentId::uuid AS parent_id,
                        c.path AS old_path,
                        CASE
                            WHEN @parentId IS NULL THEN c.identifier::ltree
                            ELSE p.path || c.identifier::ltree
                        END AS new_path,
                        (CASE
                            WHEN @parentId IS NULL THEN 0
                            ELSE p.depth + 1
                        END) - c.depth AS depth_delta
                    FROM current_department c
                    LEFT JOIN parent_department p ON TRUE
                )
                UPDATE departments d
                SET
                    "ParentId" = CASE
                        WHEN d.id = move.department_id THEN move.parent_id
                        ELSE d."ParentId"
                    END,
                    path = CASE
                        WHEN d.path = move.old_path THEN move.new_path
                        ELSE move.new_path || subpath(d.path, nlevel(move.old_path))
                    END,
                    depth = (d.depth + move.depth_delta)::smallint,
                    updated_at = now()
                FROM move, locked_subtree s
                WHERE s.id = d.id;
                """,
                new object[] { departmentIdParameter, parentIdParameter },
                cancellationToken);

            return updatedRows == 0
                ? UnitResult.Failure<Failure>(Error.Conflict("Department structure was not updated."))
                : UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ошибка при переносе отдела (DepartmentId={DepartmentId}, ParentId={ParentId})",
                departmentId,
                parentId);
            return UnitResult.Failure<Failure>(Error.Conflict("An error occurred while moving the department in the database"));
        }
    }
}
