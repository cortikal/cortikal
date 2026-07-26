using Cortikal.Core.Enums;
using Cortikal.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cortikal.Orchestrator.Agents;

public class QAEngineerAgent : BaseAgent
{
    public QAEngineerAgent(ILlmRouter llmRouter, ILogger<QAEngineerAgent> logger) : base(llmRouter, logger)
    {
    }

    public override AgentRole Role => AgentRole.QualityAssurance;

    protected override string SystemPrompt => 
        "You are the QA Engineer agent for the Cortikal swarm. " +
        "Your job is to review the generated code for bugs, security issues, missing edge cases, and test gaps. " +
        "You will receive the generated code files as input. " +
        "Output ONLY a JSON object with the following structure: " +
        "{ \"approved\": true/false, \"issues\": [ { \"filePath\": \"...\", \"severity\": \"High/Medium/Low\", \"description\": \"...\", \"suggestedFix\": \"...\" } ] }. " +
        "If you find blocking issues, set approved to false. If the code is good, set approved to true.";
}
