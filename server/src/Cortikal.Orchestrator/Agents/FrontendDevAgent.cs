using Cortikal.Core.Enums;
using Cortikal.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Cortikal.Orchestrator.Agents;

public class FrontendDevAgent : BaseAgent
{
    public FrontendDevAgent(ILlmRouter llmRouter, ILogger<FrontendDevAgent> logger) : base(llmRouter, logger)
    {
    }

    public override AgentRole Role => AgentRole.FrontendDev;

    protected override string SystemPrompt => 
        "You are the Frontend Developer agent for the Cortikal swarm. " +
        "Your job is to generate UI components, wire up API clients, and manage frontend state " +
        "based on the Architect's plan. Focus on React, Next.js, TypeScript, and CSS modules. " +
        "You will receive an execution plan containing frontend tasks. " +
        "Output ONLY a JSON array of objects, where each object has: " +
        "{ \"filePath\": \"...\", \"content\": \"...\" }. " +
        "Make sure to follow accessibility guidelines, responsive design, and robust error handling.";
}
