# Architecture Overview

## System Architecture

Cortikal is a monorepo platform composed of three main pillars:

### Frontend (`apps/web/`)
- **Framework**: Next.js 15 with App Router
- **Canvas**: React Flow for the node-based visual editor
- **State**: Zustand stores for canvas, project, and agent state
- **Styling**: Vanilla CSS with design tokens (dark-first)

### Backend (`server/`)
- **Framework**: ASP.NET Core (.NET 10)
- **Real-time**: SignalR for WebSocket communication
- **AI**: Semantic Kernel for LLM orchestration
- **Architecture**: Clean Architecture with CQRS patterns

### Desktop (`apps/desktop/`)
- **Framework**: Tauri v2 (Rust)
- **Purpose**: Local file system, terminal, and Docker access
- **Security**: Sandboxed with explicit permission grants

## Data Flow

```
User Prompt → Spark UI → Backend (NLP) → Generate arch.md
arch.md → Canvas (React Flow) → User edits → Save arch.md
arch.md → Orchestrator → Agent Swarm → Code Generation
Code → Security Scanner → Mission Control → Docker Deploy
```
