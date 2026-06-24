using Cortikal.Core.Enums;
using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Cortikal.Orchestrator.Agents;

public abstract class BaseAgent : IAgentService
{
    protected readonly Kernel _kernel;
    protected readonly ILogger _logger;
    
    public abstract AgentRole Role { get; }
    protected abstract string SystemPrompt { get; }

    protected BaseAgent(Kernel kernel, ILogger logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public virtual async Task<string> ExecuteAsync(Project project, ArchDocument architecture, string taskPrompt)
    {
        _logger.LogInformation("Agent {Role} is executing task: {Task}", Role, taskPrompt);

        // Add standard context: project details and architecture graph
        var contextStr = $"Project: {project.Name}\nArchitecture: {architecture.Nodes.Count} nodes, {architecture.Edges?.Count ?? 0} edges.\n\nTask:\n{taskPrompt}";

        try
        {
            var result = await _kernel.InvokePromptAsync(SystemPrompt + "\n\n" + contextStr);
            return result.GetValue<string>() ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent {Role} failed to execute task.", Role);
            throw;
        }
    }
}
