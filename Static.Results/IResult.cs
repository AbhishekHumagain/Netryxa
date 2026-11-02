using System.Diagnostics.CodeAnalysis;

namespace Static.Results;

public interface IResult
{
    [MemberNotNullWhen(false, nameof(Error))]
    bool IsSuccess { get; }

    Error? Error { get; }
}