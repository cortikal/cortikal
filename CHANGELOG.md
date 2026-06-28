# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Phase 3 — Prompt-to-Canvas & Canvas Completions

#### Added
- `IArchitectureGenerator` interface in `Cortikal.Core` for LLM-driven architecture generation
- `ArchitectureGeneratorService` in `Cortikal.Infrastructure` — calls OpenAI chat completions API, strips markdown fences from response, wraps raw YAML in a valid `arch.md` document, and delegates to the existing `IArchParser` to return an `ArchParseResult`
- `POST /api/canvas/generate` endpoint in `CanvasController` — accepts `{ prompt: string }`, returns `200 ArchParseResult` on success or `422` with error list on failure
- Typed `HttpClient` DI registration for `ArchitectureGeneratorService` in `Program.cs`
- `Cortikal:OpenAi:ApiKey` and `Cortikal:OpenAi:Model` config keys in `appsettings.json`
- `generateArchitecture(prompt)` method in frontend `ApiClient`
- `NodeInspector` component — floating right-side panel showing selected node details (category, type, id, position, ports, config), editable label (blur or Enter to save), and a Delete button
- Canvas toolbar now has: 📂 Import arch.md (file picker → parse API → load graph), 💾 Save as arch.md (serialize API → download), and reorganised right group
- Animated loading overlays for both template loading and AI generation states (spin/pulse keyframes)
- Generation error overlay with a Retry button

#### Changed
- `CanvasEditor` now wires the `?prompt=` query param to a real API call instead of `console.log`
- `CanvasToolbar` exports JSON and arch.md separately; import arch.md replaces manual drag-and-drop
- `serializeArchMd` parameter type changed from `any` to `unknown` in `api.ts`

---

### Added
- Initial monorepo scaffolding with Turborepo and npm workspaces
- Next.js 15 frontend with App Router and dark-themed design system
- .NET 10 backend solution with 6 projects (Api, Core, ArchParser, Orchestrator, Security, Infrastructure)
- Tauri v2 desktop app structure (placeholder)
- Shared TypeScript packages: `@cortikal/shared-types`, `@cortikal/arch-parser`, `@cortikal/ui-components`
- Complete design token system with dark/light themes
- Premium animation library with micro-interactions
- Node-specific visual styles with category color coding
- App shell with sidebar navigation, top bar, and status bar
- "The Spark" landing page with prompt input and template grid
- Placeholder pages for Canvas, Swarm, Mission Control, and Registry
- Core C# domain models: ArchDocument, Node, Edge, Port
- IArchParser interface
- Core enums: NodeCategory, AgentRole, ExecutionState
- Sample arch.md template (web-app-basic)
- Project README with badges, architecture diagram, and getting started guide
- MIT License
- Contributing guide
