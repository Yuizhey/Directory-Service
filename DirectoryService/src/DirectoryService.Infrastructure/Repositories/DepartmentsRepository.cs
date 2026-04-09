using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Departments;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            _logger.LogError(
                ex,
                "Ошибка при сохранении отдела в БД (DepartmentId={DepartmentId})",
                department.Id);
            return Result.Failure<Guid, Failure>(Error.Conflict("An error occurred while saving the department to the database"));
        }
    }
}