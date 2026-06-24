using Cortikal.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Cortikal.Orchestrator.Agents;

public class BackendDevAgent : BaseAgent
{
    public BackendDevAgent(Kernel kernel, ILogger<BackendDevAgent> logger) : base(kernel, logger)
    {
    }

    public override AgentRole Role => AgentRole.BackendDev;

    protected override string SystemPrompt => 
        "You are the Backend Developer agent. " +
        "Your job is to generate API controllers, database schemas, and business logic " +
        "based on the Architect's plan. Focus on C# and .NET Core.";
}
