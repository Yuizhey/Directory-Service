using System;
using System.Text.Json;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Application.Extensions;
using DirectoryService.Contracts.Locations.Create;
using DirectoryService.Domain.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Errors;

namespace DirectoryService.Application.Implementations.Locations.CreateLocationCommand;

public sealed record CreateLocationCommand(CreateLocationRequest location) : ICommand;

public sealed class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(n => n.location.name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(tz => tz.location.timezone)
            .MustBeValueObject(LocationTimeZone.Create);
        
        RuleFor(x => x.location.address)
            .MustBeValueObject(address =>
                LocationAddress.Create(
                    address.country,
                    address.city,
                    address.street,
                    address.houseNumber));
    }
}

public sealed class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<CreateLocationHandler> _logger;
    private readonly IValidator<CreateLocationCommand> _validator;

    public CreateLocationHandler(ILocationsRepository locationsRepository, ILogger<CreateLocationHandler> logger, IValidator<CreateLocationCommand> validator)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Failure>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if(!validationResult.IsValid)
        {
            var validationResultErrors = validationResult.Errors.Select(e => e.CustomState as Failure).SelectMany(f => f!.Errors).ToList();
            return Result.Failure<Guid, Failure>(validationResultErrors);
        }

        var locationAddress = LocationAddress.Create(command.location.address.country, command.location.address.city, command.location.address.street, command.location.address.houseNumber);
        var errors = new List<Error>();
        if (locationAddress.IsFailure)
        {
            errors.AddRange(locationAddress.Error.Errors);
        }

        var locationTimezone = LocationTimeZone.Create(command.location.timezone);
        if (locationTimezone.IsFailure)
        {
            errors.AddRange(locationTimezone.Error.Errors);
        }

        var locationName = LocationName.Create(command.location.name);
        if (locationName.IsFailure)
        {
            errors.AddRange(locationName.Error.Errors);
        }

        if (errors.Any())
        {
            _logger.LogWarning(
                "Отклонён запрос на создание локации: ошибки валидации: {Errors}",
                string.Join("; ", errors.Select(e => $"{e.Code}: {e.Message}")));
            return Result.Failure<Guid, Failure>(errors);
        }

        var result = Location.Create(locationName.Value, locationAddress.Value, locationTimezone.Value);
        var createResult = await _locationsRepository.CreateAsync(result.Value, cancellationToken);
        if (createResult.IsFailure)
        {
            _logger.LogWarning(
                "Создание локации не завершено: {Errors}",
                string.Join("; ", createResult.Error.Select(e => $"{e.Code}: {e.Message}")));
            return Result.Failure<Guid, Failure>(createResult.Error);
        }

        _logger.LogInformation(
            "Создана локация: {LocationId}, часовой пояс: {TimeZone}",
            createResult.Value,
            command.location.timezone);
        return Result.Success<Guid, Failure>(createResult.Value);
    }
}
