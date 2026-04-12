using System;
using System.Data;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.DatabaseHelpers;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Infrastructure.DatabaseHelpers;

public sealed class TransactionScope : ITransactionScope
{
    private readonly IDbTransaction _transaction;
    private readonly ILogger<TransactionScope> _logger;
    
    public TransactionScope(IDbTransaction transaction, ILogger<TransactionScope> logger)
    {
        _transaction = transaction;
        _logger = logger;
    }

    public UnitResult<Failure> Commit()
    {
        try
        {
            _transaction.Commit();
            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при коммите транзакции");
            return UnitResult.Failure<Failure>(Error.Conflict("An error occurred while committing the transaction"));
        }
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }

    public UnitResult<Failure> Rollback()
    {
        try
        {
            _transaction.Rollback();
            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при откате транзакции");
            return UnitResult.Failure<Failure>(Error.Conflict("An error occurred while rolling back the transaction"));
        }
    }
}