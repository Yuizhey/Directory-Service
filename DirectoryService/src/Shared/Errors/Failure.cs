using System;
using System.Collections;
using System.Collections.Generic;

namespace Shared.Errors;

public sealed class Failure : IEnumerable<Error>
{
    private readonly List<Error> _errors;

    public Failure(IEnumerable<Error> errors)
    {
        _errors = errors is null ? [] : [.. errors];
    }

    public IEnumerator<Error> GetEnumerator()
    {
        return _errors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

    public static implicit operator Failure(Error error) => new Failure(new[] { error });

    public static implicit operator Failure(List<Error> errors) => new Failure(errors);

    public static implicit operator Failure(Error[] errors) => new Failure(errors);
}
