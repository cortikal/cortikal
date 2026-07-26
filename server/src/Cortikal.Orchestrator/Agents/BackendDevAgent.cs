using Cortikal.Core.Enums;
using Cortikal.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Cortikal.Orchestrator.Agents;

public class BackendDevAgent : BaseAgent
{
    public BackendDevAgent(ILlmRouter llmRouter, ILogger<BackendDevAgent> logger) : base(llmRouter, logger)
    {
    }

    public override AgentRole Role => AgentRole.BackendDev;

    protected override string SystemPrompt => 
        "You are the Backend Developer agent for the Cortikal swarm. " +
        "Your job is to generate API controllers, database schemas, and business logic " +
        "based on the Architect's plan. Focus on C#, .NET Core, and Entity Framework conventions. " +
        "You will receive an execution plan containing backend tasks. " +
        "Output ONLY a JSON array of objects, where each object has: " +
        "{ \"filePath\": \"...\", \"content\": \"...\" }. " +
        "Ensure proper validation, robust error handling, and structured logging.";
}
