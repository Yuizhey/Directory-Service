using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Command;
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
    }
}

public sealed class CreatePositionHandler : ICommandHandler<Guid, CreatePositionCommand>
{
    private readonly ILogger<CreatePositionHandler> _logger;
    private readonly IPositionsRepository _positionsRepository;
    private readonly IValidator<CreatePositionCommand> _validator;

    public CreatePositionHandler(IValidator<CreatePositionCommand> validator, ILogger<CreatePositionHandler> logger, IPositionsRepository positionsRepository)
    {
        _logger = logger;
        _positionsRepository = positionsRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, Failure>> Handle(CreatePositionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var validationResultErrors = validationResult.Errors.Select(e => e.CustomState as Failure).SelectMany(f => f!.Errors).ToList();
            return Result.Failure<Guid, Failure>(validationResultErrors);
        }

        var positionName = PositionName.Create(command.request.name);
        throw new NotImplementedException();
    }
}
