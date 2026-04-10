using System;
using System.Text.Json;
using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Errors;

namespace DirectoryService.Application.Extensions;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Failure>> createValueObjectFunc)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            var result = createValueObjectFunc.Invoke(value);
            if (result.IsSuccess)
                return;

            var failure = new ValidationFailure(context.PropertyPath, "Validation failed")
            {
                CustomState = result.Error
            };

            context.AddFailure(failure);
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> ruleBuilder, Failure error)
    {
        return ruleBuilder
            .WithMessage("Validation failed")
            .WithState(_ => error);
    }
    
    public static List<Error> ToErrorList(this IList<ValidationFailure> validationFailures)
    {
        return validationFailures.Select(e => e.CustomState as Failure).SelectMany(f => f!.Errors).ToList();
    }
}
