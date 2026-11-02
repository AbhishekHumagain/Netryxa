using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Static.Results;

public sealed class Result : IResult
{
    [property: JsonPropertyOrder(1)]
    [property: JsonPropertyName("success")]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    [property: JsonPropertyOrder(2)]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Error? Error { get; }

    [JsonConstructor]
    public Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    
    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    private Result()
    {
        IsSuccess = true;
        Error = null;
    }

    public static Result Success() => new();
    public static implicit operator Result(Error error) => new(error);
    public static implicit operator Error?(Result result) => result.Error;

    public void Deconstruct(out bool isSuccess, out Error? error)
    {
        isSuccess = IsSuccess;
        error = Error;
    }

    public T Match<T>(Func<T> onSuccess, Func<Error, T> onFailure)
        => IsSuccess
            ? onSuccess()
            : onFailure(Error);

    public Task<T> MatchAsync<T>(Func<CancellationToken, Task<T>> onSuccess, Func<Error, CancellationToken, Task<T>> onFailure, CancellationToken cancellationToken = default)
        => IsSuccess
            ? onSuccess(cancellationToken)
            : onFailure(Error, cancellationToken);
}