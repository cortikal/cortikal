# Roadmap

A working checklist of what's left to build, organized by the 4 phases from `README.md`. This is the source of truth for "what to work on next" — see the `roadmap-pr-workflow` skill for how work items here turn into branches and PRs.

Status below reflects a code-level audit as of 2026-07-26; re-verify against the actual code before assuming an unchecked item is fully untouched, or a checked item is fully finished — this file should be kept honest as work lands, not aspirational.

## Phase 1 — The Spark

- [x] Prompt input landing page
- [x] Template grid / registry browsing (`RegistryController`, `registry/templates`)
- [x] Wire prompt input to `POST /api/canvas/generate` (AI-driven generation)
- [ ] Persist created projects — `ProjectController` currently holds an in-memory `List<Project>` that resets on every restart; needs real storage
- [ ] Generation error/retry UX polish beyond the current overlay

## Phase 2 — The Canvas

- [x] React Flow canvas with node/edge rendering, category color coding
- [x] Node inspector (view/edit label, delete)
- [x] Import/export `arch.md` (parse/serialize wired through `CanvasController`)
- [ ] Enforce all `arch.md` validation rules in the canvas UI itself (type-compatibility between connected ports, required-port warnings, no self-loops) — currently enforced only where the parser/validator already does
- [ ] Confirm full parity between the TS and C# parsers as the format evolves (see `arch-parser-parity` skill)

## Phase 3 — The Swarm

- [x] UI scaffolding: `AgentChatPanel`, `CodePreviewPanel`, `FileTreePanel`, `/swarm` page
- [x] SignalR wiring: `OrchestratorHub`/`AgentHub` on the backend, `orchestratorHub`/`agentHub` clients on the frontend
- [x] `BaseAgent` abstraction + `ArchitectAgent` implemented
- [ ] `FrontendDevAgent`/`BackendDevAgent` are stubs — need real prompts/behavior
- [ ] QA and DevOps agent roles don't exist yet (see `agent-role-design` skill)
- [ ] `OrchestratorStateMachine` currently **simulates** phases with `Task.Delay` calls instead of invoking real agents — this is the core "does the swarm actually work" gap
- [ ] Actual code-writing pipeline: wire agent output through `FileSystemPlugin`/`GitPlugin` to produce real files/commits in a target project
- [ ] Persist agent transcripts/history (currently only live-streamed, nothing durable)

## Phase 4 — Mission Control

- [x] UI scaffolding: `AnalyticsDashboard`, `PipelineTracker`, `/mission-control` page
- [x] `MissionControlController` with stats/pipeline endpoints
- [ ] `BuildService`/`StatsService` are mock implementations — no real Docker build or deployment logic yet
- [ ] Real log streaming during a build/deploy
- [ ] The flagship "one-click Dockerize & Deploy" — not implemented yet

## Cross-cutting

- [ ] Security scanners (`SecretScanner`, `VulnerabilityScanner`) are intentionally mocked placeholders — see `security-scanner-conventions` skill
- [ ] Test coverage gap: `Cortikal.Orchestrator.Tests` and `Cortikal.Security.Tests` only have the default scaffold test — see `dotnet-test-conventions` skill
- [ ] Frontend `ApiClient` doesn't use `@cortikal/shared-types` yet — see `api-contract-sync` skill
- [ ] Tauri desktop app is structure-only (README marks it "(placeholder)")

## How to use this roadmap

1. Pick an unchecked item (or ask the user which one to prioritize if ambiguous).
2. Follow the `roadmap-pr-workflow` skill: create a branch, do the work, open a PR.
3. When a PR merges, check the item off here in the same PR (or a fast-follow commit on `main`), and add the matching entry to `CHANGELOG.md` per the `changelog-discipline` skill.
4. If an item turns out to be bigger than expected, split it into sub-bullets here rather than silently expanding scope inside one PR.
