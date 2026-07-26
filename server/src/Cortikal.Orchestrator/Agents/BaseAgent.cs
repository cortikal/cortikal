using Cortikal.Core.Enums;
using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Cortikal.Orchestrator.Agents;

public abstract class BaseAgent : IAgentService
{
    protected readonly ILlmRouter _llmRouter;
    protected readonly ILogger _logger;
    
    public abstract AgentRole Role { get; }
    protected abstract string SystemPrompt { get; }

    protected BaseAgent(ILlmRouter llmRouter, ILogger logger)
    {
        _llmRouter = llmRouter;
        _logger = logger;
    }

    public virtual async Task<string> ExecuteAsync(Project project, ArchDocument architecture, string taskPrompt)
    {
        _logger.LogInformation("Agent {Role} is executing task: {Task}", Role, taskPrompt);

        // Add standard context: project details and architecture graph
        var contextStr = $"Project: {project.Name}\nArchitecture: {architecture.Graph.Nodes.Count} nodes, {architecture.Graph.Edges?.Count ?? 0} edges.\n\nTask:\n{taskPrompt}";

        try
        {
            var kernel = _llmRouter.GetKernelForTask(Role.ToString());
            var result = await kernel.InvokePromptAsync(SystemPrompt + "\n\n" + contextStr);
            return result.GetValue<string>() ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent {Role} failed to execute task.", Role);
            throw;
        }
    }
}
