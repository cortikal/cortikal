using Cortikal.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Cortikal.Orchestrator.Agents;

public class FrontendDevAgent : BaseAgent
{
    public FrontendDevAgent(Kernel kernel, ILogger<FrontendDevAgent> logger) : base(kernel, logger)
    {
    }

    public override AgentRole Role => AgentRole.FrontendDev;

    protected override string SystemPrompt => 
        "You are the Frontend Developer agent. " +
        "Your job is to generate UI components, wire up API clients, and manage frontend state " +
        "based on the Architect's plan. Focus on React and Next.js.";
}
