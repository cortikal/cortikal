using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;

namespace Cortikal.Orchestrator.Plugins;

public class GitPlugin
{
    private readonly ILogger<GitPlugin> _logger;
    private readonly TerminalPlugin _terminalPlugin;

    public GitPlugin(ILogger<GitPlugin> logger, TerminalPlugin terminalPlugin)
    {
        _logger = logger;
        _terminalPlugin = terminalPlugin;
    }

    [KernelFunction, Description("Commits the current changes in the project repository using Git.")]
    public async Task<string> CommitChangesAsync(
        [Description("The working directory containing the Git repository")] string repositoryPath,
        [Description("The commit message")] string commitMessage)
    {
        _logger.LogInformation("Agent committing changes in {RepositoryPath}", repositoryPath);
        
        // Use the TerminalPlugin to execute git commands
        var addOutput = await _terminalPlugin.ExecuteCommandAsync("git add -A", repositoryPath);
        if (addOutput.Contains("Error executing command") || addOutput.Contains("fatal:"))
        {
            return $"Failed to stage changes:\n{addOutput}";
        }

        var commitOutput = await _terminalPlugin.ExecuteCommandAsync($"git commit -m \"{commitMessage}\"", repositoryPath);
        return $"Git Commit Output:\n{commitOutput}";
    }
}
