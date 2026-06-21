<div align="center">

# 🧠 Cortikal

### From Prompt to Production. Visually.

An open-source, node-based **Agentic Software Engineering** platform for architecting, coding, and deploying full-stack software systems.

[![License: MIT](https://img.shields.io/badge/License-MIT-6366f1.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-15-000000.svg)](https://nextjs.org/)
[![Tauri](https://img.shields.io/badge/Tauri-v2-FFC131.svg)](https://tauri.app/)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-8b5cf6.svg)](http://makeapullrequest.com)

---

**Describe your architecture → AI agents build it → Deploy with one click.**

[Getting Started](#-getting-started) •
[Architecture](#-architecture) •
[The 4 Phases](#-the-4-phases) •
[Tech Stack](#-tech-stack) •
[Contributing](#-contributing)

</div>

---

## ✨ What is Cortikal?

Cortikal is a **visual operating system for AI-driven software development**. Think of it as n8n meets Cursor — but specifically designed for architecting, generating, and deploying full-stack systems.

Instead of chatting with AI line-by-line, you:

1. **Describe** your project or browse community architecture templates
2. **Visualize** the architecture as an interactive node graph
3. **Watch** AI agents collaborate to write your entire codebase
4. **Deploy** everything with a single click

### 🎯 Key Features

- **🔮 Architecture-as-Code (`arch.md`)** — Define architectures in a markdown-based format that renders natively on GitHub
- **🎨 Visual Canvas** — Drag-and-drop node editor with strict I/O type contracts between components
- **🤖 Multi-Agent Swarm** — Specialized AI agents (Architect, Frontend Dev, Backend Dev, QA, DevOps) collaborate in real-time
- **🚀 One-Click Deploy** — Dockerize and deploy from a dark-themed Mission Control dashboard
- **🔐 Security-First** — Automatic secret scanning, vulnerability detection, and code quality analysis
- **🧩 Architecture Registry** — Community-driven library of reusable architecture templates
- **🔄 Multi-LLM Routing** — Intelligent routing between proprietary and fine-tuned models

---

## 🏗 Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Cortikal Platform                     │
├──────────────┬──────────────────┬────────────────────────┤
│   Frontend   │    Desktop App   │   Backend Orchestrator │
│  (Next.js)   │    (Tauri v2)    │      (.NET 10)         │
│              │                  │                        │
│ • React Flow │ • File System    │ • Custom State Machine │
│ • Canvas     │ • Terminal       │ • Semantic Kernel      │
│ • Spark UI   │ • Docker CLI     │ • Multi-LLM Router    │
│ • Swarm View │ • Git Ops        │ • Vector Memory       │
│ • Mission    │                  │ • Security Scanner    │
│   Control    │                  │ • Agent Framework     │
└──────────────┴──────────────────┴────────────────────────┘
```

---

## 🎬 The 4 Phases

### ⚡ Phase 1 — The Spark
A minimalist prompt input where you describe your project in natural language, or browse the community architecture registry to find a template.

### 🎨 Phase 2 — The Canvas
An interactive, node-based visual diagram. Drag frontend, backend, database, and infrastructure nodes. Connect them with typed data flow edges. Save as `arch.md`.

### 🤖 Phase 3 — The Swarm (Agent Studio)
Watch multi-agent AI teams collaborate in real-time. A Lead Architect plans, Frontend and Backend Devs code, QA reviews, and DevOps deploys — all streaming live.

### 🚀 Phase 4 — Mission Control
A dark-themed DevOps dashboard. One-click "Dockerize & Deploy" with live terminal output, build pipeline visualization, and structured log viewing.

---

## 🛠 Tech Stack

| Layer | Technology | Why |
|-------|-----------|-----|
| **Frontend** | Next.js 15, React Flow | Modern SSR framework + best-in-class node editor |
| **Desktop** | Tauri v2 (Rust) | Lightweight, secure local file & CLI access |
| **Backend** | .NET 10, C# | Type safety, high throughput, enterprise stability |
| **AI Orchestration** | Semantic Kernel | Microsoft's production-grade AI SDK for .NET |
| **State Management** | Zustand | Minimal, fast, no boilerplate |
| **Monorepo** | Turborepo + npm workspaces | Efficient monorepo builds and caching |

---

## 🚀 Getting Started

### Prerequisites

- **Node.js** ≥ 20.x
- **.NET** ≥ 10.0
- **Rust** ≥ 1.75 (for Tauri desktop builds)

### Quick Start

```bash
# Clone the repository
git clone https://github.com/cortikal/cortikal.git
cd cortikal

# Install dependencies
npm install

# Start the frontend dev server
cd apps/web && npm run dev

# (In another terminal) Start the .NET backend
cd server/src/Cortikal.Api && dotnet run
```

### Project Structure

```
cortikal/
├── apps/
│   ├── web/            # Next.js frontend (React Flow canvas)
│   └── desktop/        # Tauri v2 desktop wrapper
├── packages/
│   ├── arch-parser/    # TypeScript arch.md parser
│   ├── shared-types/   # Shared TypeScript type definitions
│   └── ui-components/  # Shared React component library
├── server/             # .NET 10 backend orchestrator
│   ├── src/
│   │   ├── Cortikal.Api/          # ASP.NET Web API
│   │   ├── Cortikal.Core/        # Domain models & interfaces
│   │   ├── Cortikal.ArchParser/  # C# arch.md parser
│   │   ├── Cortikal.Orchestrator/# State machine & agents
│   │   ├── Cortikal.Security/    # Security scanning
│   │   └── Cortikal.Infrastructure/ # External integrations
│   └── tests/
├── registry/           # Community arch.md templates
└── docs/               # Documentation
```

---

## 📝 The `arch.md` Format

Cortikal uses `arch.md` — **Architecture-as-Code** — a markdown-based format that renders natively on GitHub:

```markdown
---
name: E-Commerce Platform
author: cortikal
version: 1.0.0
tags: [ecommerce, microservices, react, dotnet]
complexity: complex
description: A full-stack e-commerce platform with microservices
---

# E-Commerce Platform Architecture

A scalable e-commerce system with separate services for products,
orders, and payments.

​```arch
nodes:
  - id: frontend
    type: react-app
    category: frontend
    label: "Storefront (React)"
    position: { x: 100, y: 200 }

  - id: api-gateway
    type: dotnet-api
    category: backend
    label: "API Gateway (.NET)"
    position: { x: 400, y: 200 }

  - id: database
    type: postgresql
    category: database
    label: "Product Database"
    position: { x: 700, y: 200 }

edges:
  - id: e1
    sourceNodeId: frontend
    targetNodeId: api-gateway
    dataType: http
    edgeType: dataflow

  - id: e2
    sourceNodeId: api-gateway
    targetNodeId: database
    dataType: sql
    edgeType: dataflow
​```
```

---

## 🤝 Contributing

We welcome contributions! Cortikal is built in the open and we love pull requests.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please read our [Contributing Guide](docs/guides/contributing.md) for details.

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

**Built with 🧠 by the Cortikal community**

[⬆ Back to top](#-cortikal)

</div>