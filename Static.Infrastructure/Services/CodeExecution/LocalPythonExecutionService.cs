using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Static.Application.Common.Interfaces;

namespace Static.Infrastructure.Services.CodeExecution;

/// <summary>
/// Executes Python code locally using the python3 interpreter installed in the container.
/// Runs with a 10-second wall-clock timeout and captures both stdout and stderr.
/// </summary>
internal sealed class LocalPythonExecutionService(
    ILogger<LocalPythonExecutionService> logger, IConfiguration configuration) : ICodeExecutionService
{
    private const int TimeoutSeconds = 10;

    public async Task<CodeExecutionResult> ExecuteAsync(
        string code, string language, CancellationToken ct = default)
    {
        if (!configuration.GetValue<bool>("CodeExecution:EnableUnsafeLocalRunner"))
            throw new InvalidOperationException("Local code execution is disabled. Configure an isolated execution service before enabling this feature.");
        var tempFile = Path.Combine(Path.GetTempPath(), $"playground_{Guid.NewGuid():N}.py");

        try
        {
            await File.WriteAllTextAsync(tempFile, code, ct);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(TimeoutSeconds));

            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName               = "python3",
                Arguments              = $"\"{tempFile}\"",
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true,
                WorkingDirectory       = Path.GetTempPath()
            };

            process.Start();

            // Read output and wait concurrently to avoid deadlocks on large output
            var stdoutTask = process.StandardOutput.ReadToEndAsync(cts.Token);
            var stderrTask = process.StandardError.ReadToEndAsync(cts.Token);

            await process.WaitForExitAsync(cts.Token);

            var stdout = await stdoutTask;
            var stderr = await stderrTask;

            logger.LogDebug("Python executed, exit={Code}, stdout={Len}b", process.ExitCode, stdout.Length);

            return new CodeExecutionResult(
                Output: stdout,
                Error: stderr,
                ExitCode: process.ExitCode,
                Language: "python",
                Version: "3.x");
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning("Python execution timed out after {Seconds}s", TimeoutSeconds);
            return new CodeExecutionResult(
                Output: string.Empty,
                Error: $"Execution timed out after {TimeoutSeconds} seconds.",
                ExitCode: -1,
                Language: "python",
                Version: "3.x");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Python execution failed unexpectedly");
            return new CodeExecutionResult(
                Output: string.Empty,
                Error: $"Execution error: {ex.Message}",
                ExitCode: -1,
                Language: "python",
                Version: "3.x");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
