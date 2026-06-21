/**
 * Cortikal — Agent Type Definitions
 *
 * Defines agent roles, message types, and state machine
 * types for the multi-agent orchestration system.
 */

/** Agent roles in the development swarm */
export enum AgentRole {
  LeadArchitect = "lead_architect",
  FrontendDev = "frontend_dev",
  BackendDev = "backend_dev",
  QualityAssurance = "quality_assurance",
  DevOps = "devops",
  SecurityAuditor = "security_auditor",
}

/** Agent display metadata */
export interface AgentInfo {
  role: AgentRole;
  displayName: string;
  avatar: string;
  description: string;
  color: string;
}

/** Message types in agent communication */
export enum MessageType {
  Plan = "plan",
  Code = "code",
  Review = "review",
  Question = "question",
  Approval = "approval",
  Error = "error",
  Status = "status",
}

/** A message in the agent communication stream */
export interface AgentMessage {
  id: string;
  timestamp: string;
  fromAgent: AgentRole;
  toAgent?: AgentRole;
  type: MessageType;
  content: string;
  metadata?: Record<string, unknown>;
  /** If type is 'code', the associated file path */
  filePath?: string;
  /** If type is 'code', the language for syntax highlighting */
  language?: string;
}

/** Orchestrator state machine states */
export enum ExecutionState {
  Idle = "idle",
  Planning = "planning",
  Generating = "generating",
  Reviewing = "reviewing",
  Deploying = "deploying",
  Complete = "complete",
  Error = "error",
  Paused = "paused",
}

/** Current state of the orchestration pipeline */
export interface OrchestratorState {
  currentState: ExecutionState;
  activeAgents: AgentRole[];
  progress: number;
  currentTask?: string;
  error?: string;
}
