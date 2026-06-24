using Cortikal.Core.Enums;
using Cortikal.Core.Models;

namespace Cortikal.Core.Interfaces;

public interface IAgentService
{
    AgentRole Role { get; }
    
    /// <summary>
    /// Executes the agent's task based on the current project and architecture.
    /// </summary>
    Task<string> ExecuteAsync(Project project, ArchDocument architecture, string taskPrompt);
}
