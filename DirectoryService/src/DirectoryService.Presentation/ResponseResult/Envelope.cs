using System;
using System.Text.Json.Serialization;
using Shared.Errors;

namespace DirectoryService.Presentation;

public sealed record Envelope
{
    public object? Result { get; }
    public Failure? Errors { get; }
    public DateTime GeneratedAt { get; }
    public bool HasErrors => Errors != null && Errors.Any();

    [JsonConstructor]
    private Envelope(object? result, Failure? errors)
    {
        Result = result;
        Errors = errors;
        GeneratedAt = DateTime.UtcNow;
    }

    public static Envelope Success(object? result = null) 
    {
        return new Envelope(result, null);
    }

    public static Envelope Failure(Failure errors)
    {
        return new Envelope(null, errors);
    }
}

public sealed record Envelope<T>
{
    public T? Result { get; }
    public Failure? Errors { get; }
    public DateTime GeneratedAt { get; }
    public bool HasErrors => Errors != null && Errors.Any();

    [JsonConstructor]
    private Envelope(T? result, Failure? errors)
    {
        Result = result;
        Errors = errors;
        GeneratedAt = DateTime.UtcNow;
    }

    public static Envelope<T> Success(T? result = default) 
    {
        return new Envelope<T>(result, null);
    }

    public static Envelope<T> Failure(Failure errors)
    {
        return new Envelope<T>(default, errors);
    }
}