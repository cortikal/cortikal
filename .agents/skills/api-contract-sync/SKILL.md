---
name: api-contract-sync
description: Use whenever changing a Cortikal.Api controller or SignalR hub, or the frontend code that calls them (apps/web/src/lib/api.ts, packages/shared-types). Keeps the .NET API surface and the Next.js client from silently drifting apart.
---

# Keeping the API contract in sync across .NET and Next.js

Cortikal's frontend (`apps/web`) talks to the .NET backend (`Cortikal.Api`) two ways:

1. **REST**, via `ApiClient` in `apps/web/src/lib/api.ts`, calling controllers in `server/src/Cortikal.Api/Controllers/` (`CanvasController`, `MissionControlController`, `ProjectController`, `RegistryController`).
2. **SignalR**, via `OrchestratorHubClient`/`AgentHubClient` in the same file, connecting to `server/src/Cortikal.Api/Hubs/OrchestratorHub.cs` and `AgentHub.cs`.

There is also `packages/shared-types/src/` (`agent-types.ts`, `graph-types.ts`, `node-types.ts`, `registry-types.ts`) which exists specifically to hold TypeScript types that should mirror the C# `Cortikal.Core.Models`/`Enums` shapes.

## Known existing drift — be aware, don't make it worse

As of this writing, `apps/web/src/lib/api.ts` does **not** import from `@cortikal/shared-types` — it uses inline `unknown`/ad-hoc object shapes (e.g. the hand-written return type in `generateArchitecture`) instead of the shared types package. Don't treat this as the intended pattern to copy. When you touch `api.ts`, prefer importing the matching type from `@cortikal/shared-types` if one already exists for that response shape, and add one there if it doesn't, rather than adding another inline `unknown`-typed shape.

## When you change a controller endpoint

1. Update the corresponding method in `ApiClient` (`apps/web/src/lib/api.ts`) in the same change — matching HTTP method, route, request body shape, and response shape.
2. If the endpoint's request/response DTO is a C# `record`/class (e.g. `GenerateRequest` in `CanvasController.cs`, or `ArchParseResult`), check whether an equivalent type exists in `packages/shared-types/src/`. If it doesn't, add one instead of letting the frontend describe the shape ad hoc.
3. Match existing error-handling conventions on the frontend: current `ApiClient` methods check `res.ok` and `throw new Error(...)` with a fixed message, except `generateArchitecture`, which parses the JSON body first and extracts `data?.errors?.[0] ?? data?.error ?? "..."` because the backend returns a structured `422 UnprocessableEntity` body on failure (see `CanvasController.GenerateFromPrompt`). If your new/changed endpoint can return a structured error body (not just a bare error string), follow the `generateArchitecture` pattern rather than the simpler `res.ok` check, so the frontend surfaces the real backend error instead of a generic one.
4. Keep the `[HttpPost]`/`[Route]` conventions already used in the controllers (`[ApiController]`, `[Route("api/[controller]")]`, action attributes like `[HttpPost("generate")]`) — new endpoints should follow the same routing shape so `API_BASE` (`http://localhost:5050/api`) path construction in `api.ts` stays predictable.

## When you change a SignalR hub

1. `OrchestratorHub`/`AgentHub` server-side methods and client event names (e.g. `"ReceiveStateUpdate"`, `"ReceiveAgentMessage"`) must match exactly what `OrchestratorHubClient.onStateUpdate` / `AgentHubClient.onAgentMessage` subscribe to in `api.ts`. A rename on one side without the other fails silently at runtime (no compile error), so grep both sides before finishing.
2. If the payload shape of a hub event changes, update the callback signature in the relevant `on*` method in `api.ts` and, if a shared type exists/should exist for that payload, keep it in `packages/shared-types`.

## Process

1. Before changing an endpoint or hub, search for all call sites in `apps/web/src/lib/api.ts` and any component/store that calls `ApiClient`/`orchestratorHub`/`agentHub` directly.
2. Make the backend and frontend changes together in the same task — don't land one side and defer the other.
3. If `Cortikal.Api.http` (the REST Client scratch file in `Cortikal.Api/`) has example requests for the endpoint you're changing, update it too so it stays a reliable manual-testing reference.
