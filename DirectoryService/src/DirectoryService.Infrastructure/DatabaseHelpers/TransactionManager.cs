using System;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.DatabaseHelpers;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Infrastructure.DatabaseHelpers;

public sealed class TransactionManager : ITransactionManager
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<TransactionManager> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public TransactionManager(DirectoryServiceDbContext dbContext, ILogger<TransactionManager> logger, ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public async Task<Result<ITransactionScope, Failure>> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var beginTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            var logger = _loggerFactory.CreateLogger<TransactionScope>();
            var transaction = beginTransaction.GetDbTransaction();
            var transactionScope = new TransactionScope(transaction, logger);
            return Result.Success<ITransactionScope, Failure>(transactionScope);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при начале транзакции");
            return Result.Failure<ITransactionScope, Failure>(Error.ServiceUnavailable("An error occurred while beginning the transaction"));
        }
    }

    public async Task<UnitResult<Failure>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении изменений в БД");
            return UnitResult.Failure<Failure>(Error.ServiceUnavailable("An error occurred while saving changes to the database"));
        }
    }
}