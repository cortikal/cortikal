using Cortikal.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Cortikal.Orchestrator.Agents;

public class ArchitectAgent : BaseAgent
{
    public ArchitectAgent(Kernel kernel, ILogger<ArchitectAgent> logger) : base(kernel, logger)
    {
    }

    public override AgentRole Role => AgentRole.LeadArchitect;

    protected override string SystemPrompt => 
        "You are the Lead Architect for the Cortikal agent swarm. " +
        "Your job is to review the user's natural language request and the current architecture graph (if any). " +
        "You will design the software architecture and delegate tasks to the FrontendDev and BackendDev agents. " +
        "Output a detailed execution plan.";
}
