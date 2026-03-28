using System.Diagnostics;
using System.Text;

namespace CICD_App.Services;

public class CommandExecutor : ICommandExecutor
{
    private readonly ILoggerService _logger;

    public CommandExecutor(ILoggerService logger)
    {
        _logger = logger;
    }

    public async Task<int> ExecuteCommandAsync(string command, string arguments, string workingDirectory)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processStartInfo };

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        string output = outputBuilder.ToString();
        string error = errorBuilder.ToString();

        if (!string.IsNullOrWhiteSpace(output))
        {
            _logger.LogInfo($"Output: {output.Trim()}");
        }

        if (!string.IsNullOrWhiteSpace(error))
        {
            _logger.LogWarning($"Error output: {error.Trim()}");
        }

        return process.ExitCode;
    }
}