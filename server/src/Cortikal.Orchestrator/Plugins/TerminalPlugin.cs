using System.ComponentModel;
using System.Diagnostics;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;

namespace Cortikal.Orchestrator.Plugins;

public class TerminalPlugin
{
    private readonly ILogger<TerminalPlugin> _logger;

    public TerminalPlugin(ILogger<TerminalPlugin> logger)
    {
        _logger = logger;
    }

    [KernelFunction, Description("Executes a terminal/shell command and returns the output.")]
    public async Task<string> ExecuteCommandAsync(
        [Description("The shell command to execute")] string command,
        [Description("The working directory for the command")] string workingDirectory)
    {
        _logger.LogInformation("Agent executing command: {Command} in {WorkingDirectory}", command, workingDirectory);
        
        try
        {
            // Simple Process execution wrapper
            // In a real implementation this would need security sandboxing
            var processInfo = new ProcessStartInfo
            {
                FileName = OperatingSystem.IsWindows() ? "cmd.exe" : "/bin/bash",
                Arguments = OperatingSystem.IsWindows() ? $"/c \"{command}\"" : $"-c \"{command}\"",
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null) return "Failed to start process.";

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            var output = await outputTask;
            var error = await errorTask;

            return $"Exit Code: {process.ExitCode}\nOutput:\n{output}\nError:\n{error}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute command.");
            return $"Error executing command: {ex.Message}";
        }
    }
}
