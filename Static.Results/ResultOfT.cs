using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Static.Results;

public sealed class Result<TValue> : IResult
{
    [property: JsonPropertyOrder(1)]
    [property: JsonPropertyName("success")]
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    [property: JsonPropertyOrder(2)]
    [property: JsonPropertyName("error")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Error? Error { get; }

    [property: JsonPropertyOrder(3)]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [property: JsonPropertyName("data")]
    public TValue? Value { get; }

    [JsonConstructor]
    public Result(bool isSuccess, Error? error, TValue? value)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
        Value = default;
    }

    private Result(TValue value)
    {
        IsSuccess = true;
        Error = null;
        Value = value;
    }

    public static Result<TValue> From(Error error) => new(error);
    public static Result<TValue> From(TValue value) => new(value);

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Error error) => new(error);
    public static implicit operator Error?(Result<TValue> result) => result.Error;
    public static implicit operator TValue?(Result<TValue> result) => result.Value;

    public void Deconstruct(out bool isSuccess, out Error? error, out TValue? value)
    {
        isSuccess = IsSuccess;
        error = Error;
        value = Value;
    }

    public T Match<T>(Func<TValue, T> onSuccess, Func<Error, T> onFailure)
        => IsSuccess
            ? onSuccess(Value)
            : onFailure(Error);

    public Task<T> MatchAsync<T>(Func<TValue, CancellationToken, Task<T>> onSuccess,
        Func<Error, CancellationToken, Task<T>> onFailure,
        CancellationToken cancellationToken = default)
        => IsSuccess
            ? onSuccess(Value, cancellationToken)
            : onFailure(Error, cancellationToken);
}