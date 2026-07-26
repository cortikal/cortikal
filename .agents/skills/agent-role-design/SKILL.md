---
name: agent-role-design
description: Use whenever adding or modifying an AI agent role (Architect, FrontendDev, BackendDev, QA, DevOps) under server/src/Cortikal.Orchestrator/Agents, wiring it into the state machine, or giving it new plugins/tools. Keeps the Swarm's agent framework consistent as Phase 3 is built out.
---

# Designing agent roles in the Orchestrator

Cortikal's "Swarm" is built on Semantic Kernel inside `server/src/Cortikal.Orchestrator/`. As of this writing it has a real `BaseAgent` abstraction and an `ArchitectAgent`, with `FrontendDevAgent`/`BackendDevAgent` stubs, plus a `OrchestratorStateMachine` that is currently a **placeholder loop** (it just does `Task.Delay` calls between states, not real agent invocation yet — see `RunStateMachineLoop`). When you build this out, follow the existing shape rather than introducing a parallel pattern.

## Existing structure to follow

- `Agents/BaseAgent.cs` — abstract base implementing `IAgentService` (`Cortikal.Core.Interfaces`). Exposes `Role` (an `AgentRole` enum value) and `SystemPrompt`, and a virtual `ExecuteAsync(Project, ArchDocument, string taskPrompt)` that builds a context string (project name + node/edge counts + task) and calls `_kernel.InvokePromptAsync(...)`.
- Concrete agents (`ArchitectAgent`, `FrontendDevAgent`, `BackendDevAgent`) are thin: they set `Role` and a `SystemPrompt` string, and take `Kernel` + `ILogger<T>` via DI. `ArchitectAgent`'s prompt is a good reference for tone/scope: it states the agent's job, what context it gets, and what it should output.
- `AgentRole` enum lives in `Cortikal.Core.Enums` — add new roles there, not as ad-hoc strings.
- `Plugins/` (`FileSystemPlugin.cs`, `GitPlugin.cs`, `TerminalPlugin.cs`) are Semantic Kernel plugins agents can invoke as tools. New agent capabilities (e.g. a Docker/deploy plugin for a DevOps agent) belong here, registered with the `Kernel` via DI in `Cortikal.Api/Program.cs`, following the existing plugin registration pattern.
- `StateMachine/OrchestratorStateMachine.cs` implements `IOrchestrator`, tracks `ExecutionState` (`Idle`, `Planning`, `Generating`, `Reviewing`, `Paused`, `Complete`, `Error`), and raises `StateChanged` events consumed by `Cortikal.Api/Hubs/OrchestratorHub.cs` (SignalR) and surfaced to the frontend via `orchestratorHub`/`agentHub` in `apps/web/src/lib/api.ts`.
- `Services/` (`BuildService.cs`, `StatsService.cs`) hold cross-cutting orchestration services, not agent-specific logic.

## When adding a new agent role

1. Add the role to `Cortikal.Core.Enums.AgentRole` if it doesn't exist (e.g. `QAEngineer`, `DevOpsEngineer`).
2. Create `Agents/<Role>Agent.cs` extending `BaseAgent`, following the `ArchitectAgent` shape: constructor takes `(Kernel kernel, ILogger<T> logger)`, override `Role` and `SystemPrompt`.
3. Write the `SystemPrompt` to be specific about: (a) the agent's single responsibility in the swarm, (b) what inputs it receives (it always gets project + architecture graph counts + a task prompt via `BaseAgent.ExecuteAsync`), (c) what output format downstream consumers expect (plan text? code diff? structured JSON?). If the output needs to be structured rather than free text, consider overriding `ExecuteAsync` rather than relying on the base implementation's raw string return.
4. Register the new agent for DI in `Cortikal.Api/Program.cs` alongside the existing agents.
5. If the agent needs new tool access, add a plugin under `Plugins/` and register it with the `Kernel` builder, rather than hand-rolling file/git/terminal access inside the agent class itself.
6. Wire the new role into `OrchestratorStateMachine`'s transitions if it participates in the main pipeline (Planning → Generating → Reviewing → Complete) — replace placeholder `Task.Delay` calls with real calls to `IAgentService.ExecuteAsync` for that state, and keep raising `StateChanged` with a human-readable `message` describing what's happening, since the frontend Swarm view surfaces that message live.
7. Add or update tests in `server/tests/Cortikal.Orchestrator.Tests/` for the new agent and any state machine transitions it participates in (see the `dotnet-test-conventions` skill — this test project currently only has the default `UnitTest1.cs` scaffold, so there's no strong existing pattern to copy yet; establish one deliberately rather than leaving it as boilerplate).

## Things to avoid

- Don't bypass `BaseAgent`/`IAgentService` to call `Kernel` directly from a controller or the state machine — keep agent invocation behind the agent classes so behavior stays testable and swappable.
- Don't hardcode role names as strings where `AgentRole` enum values should be used — the frontend's Swarm view and SignalR messages should stay driven by the same enum, not parallel string literals.
- Don't give every agent access to every plugin by default — only wire the plugins a given role actually needs (e.g. a `FrontendDevAgent` probably needs `FileSystemPlugin`/`GitPlugin`, not necessarily raw `TerminalPlugin` access, unless that's a deliberate design decision).
