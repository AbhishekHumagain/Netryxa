using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Static.Application.Common.Interfaces;

namespace Static.Infrastructure.Services.CodeExecution;

/// <summary>
/// Executes code snippets via the Piston sandboxed execution API (https://emkc.org/api/v2/piston).
/// No API key required. Supports Python, JavaScript, and many other languages.
/// </summary>
internal sealed class PistonCodeExecutionService(
    HttpClient httpClient,
    ILogger<PistonCodeExecutionService> logger) : ICodeExecutionService
{
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task<CodeExecutionResult> ExecuteAsync(
        string code, string language, CancellationToken ct = default)
    {
        var request = new PistonRequest(
            Language: language.ToLower(),
            Version: "*",
            Files: [new PistonFile("main.py", code)],
            Stdin: string.Empty,
            Args: [],
            RunTimeout: 5_000,
            CompileTimeout: 10_000,
            RunMemoryLimit: 128 * 1024 * 1024);

        logger.LogDebug("Executing {Language} code via Piston ({Length} chars)", language, code.Length);

        var response = await httpClient.PostAsJsonAsync(
            "https://emkc.org/api/v2/piston/execute", request, _json, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PistonResponse>(_json, ct)
                     ?? throw new InvalidOperationException("Empty response from Piston API.");

        logger.LogDebug("Piston executed {Language} {Version}, exit={Code}",
            result.Language, result.Version, result.Run.Code);

        return new CodeExecutionResult(
            Output: result.Run.Stdout,
            Error: result.Run.Stderr,
            ExitCode: result.Run.Code,
            Language: result.Language,
            Version: result.Version);
    }

    // ── Piston request/response models ────────────────────────────────────────

    private sealed record PistonRequest(
        string Language,
        string Version,
        List<PistonFile> Files,
        string Stdin,
        string[] Args,
        int RunTimeout,
        int CompileTimeout,
        long RunMemoryLimit);

    private sealed record PistonFile(string Name, string Content);

    private sealed record PistonResponse(
        string Language,
        string Version,
        PistonRun Run);

    private sealed record PistonRun(
        string Stdout,
        string Stderr,
        string Output,
        int Code,
        [property: JsonPropertyName("signal")] string? Signal);
}
