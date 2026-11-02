namespace Static.Application.Common.Interfaces;

public interface ICodeExecutionService
{
    Task<CodeExecutionResult> ExecuteAsync(string code, string language, CancellationToken ct = default);
}

public sealed record CodeExecutionResult(
    string Output,
    string Error,
    int ExitCode,
    string Language,
    string Version);
