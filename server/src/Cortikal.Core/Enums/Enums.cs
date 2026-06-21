namespace Cortikal.Core.Enums;

/// <summary>
/// Node categories for the visual canvas.
/// </summary>
public enum NodeCategory
{
    Frontend,
    Backend,
    Database,
    Infrastructure,
    AI,
    Integration,
    Custom
}

/// <summary>
/// Agent roles in the development swarm.
/// </summary>
public enum AgentRole
{
    LeadArchitect,
    FrontendDev,
    BackendDev,
    QualityAssurance,
    DevOps,
    SecurityAuditor
}

/// <summary>
/// Orchestrator state machine states.
/// </summary>
public enum ExecutionState
{
    Idle,
    Planning,
    Generating,
    Reviewing,
    Deploying,
    Complete,
    Error,
    Paused
}
