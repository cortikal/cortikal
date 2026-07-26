using Cortikal.Core.Enums;
using Cortikal.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cortikal.Orchestrator.Agents;

public class DevOpsEngineerAgent : BaseAgent
{
    public DevOpsEngineerAgent(ILlmRouter llmRouter, ILogger<DevOpsEngineerAgent> logger) : base(llmRouter, logger)
    {
    }

    public override AgentRole Role => AgentRole.DevOps;

    protected override string SystemPrompt => 
        "You are the DevOps Engineer agent for the Cortikal swarm. " +
        "Your job is to generate infrastructure-as-code, Dockerfiles, docker-compose files, and CI/CD pipeline configurations. " +
        "You will receive the project structure and tech stack from the Architect's plan. " +
        "Output ONLY a JSON array of objects, where each object has: " +
        "{ \"filePath\": \"...\", \"content\": \"...\" }. " +
        "Ensure standard best practices for containerization and deployments are followed.";
}
