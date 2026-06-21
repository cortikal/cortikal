# arch.md — Architecture-as-Code Specification

**Version**: 1.0.0
**Status**: Draft
**Authors**: Cortikal Contributors

---

## Overview

`arch.md` is a markdown-based file format for defining software architecture graphs. It is designed to:

1. **Render natively** on GitHub, GitLab, and any markdown viewer
2. **Be human-readable** — authors can write and edit architectures by hand
3. **Be machine-parseable** — tools like Cortikal can parse, validate, and render the graph
4. **Support strict I/O contracts** — every connection between nodes has typed data

## File Structure

An `arch.md` file consists of three sections in order:

```
┌─────────────────────────────────────┐
│  1. YAML Frontmatter (required)     │
│     --- delimited metadata block    │
├─────────────────────────────────────┤
│  2. Markdown Description (optional) │
│     Free-form markdown text         │
├─────────────────────────────────────┤
│  3. Architecture Graph (required)   │
│     ```arch fenced code block       │
│     containing YAML graph data      │
└─────────────────────────────────────┘
```

---

## 1. YAML Frontmatter

The file **must** begin with a YAML frontmatter block delimited by `---`:

```yaml
---
name: "E-Commerce Platform"        # Required. Architecture name.
author: "cortikal"                  # Required. Author or organization.
version: "1.0.0"                    # Required. Semantic version string.
tags: [ecommerce, react, dotnet]    # Required. Array of search tags.
complexity: "complex"               # Required. One of: simple, moderate, complex, enterprise.
description: "A scalable..."        # Required. Short description (1-2 sentences).
createdAt: "2025-01-01T00:00:00Z"   # Optional. ISO 8601 creation timestamp.
updatedAt: "2025-06-15T12:00:00Z"   # Optional. ISO 8601 last modified timestamp.
---
```

### Metadata Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `name` | `string` | ✅ | Human-readable architecture name |
| `author` | `string` | ✅ | Author name or organization |
| `version` | `string` | ✅ | Semantic version (e.g., `"1.0.0"`) |
| `tags` | `string[]` | ✅ | Searchable tags for the registry |
| `complexity` | `enum` | ✅ | One of: `simple`, `moderate`, `complex`, `enterprise` |
| `description` | `string` | ✅ | Brief description of the architecture |
| `createdAt` | `string` | ❌ | ISO 8601 timestamp |
| `updatedAt` | `string` | ❌ | ISO 8601 timestamp |

### Complexity Levels

| Level | Description | Typical Node Count |
|-------|-------------|-------------------|
| `simple` | Single-tier or basic client-server | 1–4 nodes |
| `moderate` | Multi-tier with caching or queues | 4–8 nodes |
| `complex` | Microservices, multiple databases | 8–15 nodes |
| `enterprise` | Large-scale distributed systems | 15+ nodes |

---

## 2. Markdown Description (Optional)

Between the frontmatter and the `arch` code block, authors may include any valid markdown. This renders as documentation on GitHub and provides context for the architecture.

```markdown
# E-Commerce Platform Architecture

This architecture implements a scalable e-commerce system with:
- **React** storefront for the customer-facing UI
- **.NET API Gateway** handling authentication and routing
- **PostgreSQL** for product and order data
- **Redis** for session caching and rate limiting
```

---

## 3. Architecture Graph

The graph is defined inside a fenced code block with the language identifier `arch`:

````markdown
```arch
nodes:
  - id: frontend
    type: react-app
    ...

edges:
  - id: e1
    sourceNodeId: frontend
    ...
```
````

The content of the `arch` block is **YAML** defining two top-level arrays: `nodes` and `edges`.

### 3.1 Node Schema

Each node represents a software component in the architecture.

```yaml
nodes:
  - id: "frontend"                    # Required. Unique identifier (kebab-case).
    type: "react-app"                 # Required. Node type from the type registry.
    category: "frontend"              # Required. One of the NodeCategory values.
    label: "Storefront (React)"       # Required. Human-readable display label.
    position:                         # Required. Canvas position.
      x: 100
      y: 200
    config:                           # Optional. Type-specific configuration.
      framework: "nextjs"
      port: 3000
    inputs:                           # Optional. Input port definitions.
      - id: "http-in"
        label: "HTTP Requests"
        direction: "input"
        dataType: "http"
        required: true
        description: "Incoming HTTP requests"
    outputs:                          # Optional. Output port definitions.
      - id: "http-out"
        label: "HTTP Responses"
        direction: "output"
        dataType: "http"
        required: true
```

#### Node Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `id` | `string` | ✅ | Unique node ID (kebab-case recommended) |
| `type` | `string` | ✅ | Node type identifier |
| `category` | `enum` | ✅ | `frontend`, `backend`, `database`, `infrastructure`, `ai`, `integration`, `custom` |
| `label` | `string` | ✅ | Display label |
| `position` | `object` | ✅ | `{ x: number, y: number }` canvas coordinates |
| `config` | `object` | ❌ | Key-value configuration specific to the node type |
| `inputs` | `Port[]` | ❌ | Array of input port definitions |
| `outputs` | `Port[]` | ❌ | Array of output port definitions |

#### Node Categories

| Category | Color | Description | Example Types |
|----------|-------|-------------|---------------|
| `frontend` | Cyan | Client-side applications | `react-app`, `vue-app`, `angular-app`, `static-site` |
| `backend` | Violet | Server-side services | `dotnet-api`, `nodejs-api`, `python-api`, `go-service` |
| `database` | Amber | Data persistence | `postgresql`, `mongodb`, `redis`, `elasticsearch` |
| `infrastructure` | Green | Infrastructure & DevOps | `docker`, `kubernetes`, `nginx`, `aws-lambda` |
| `ai` | Pink | AI/ML components | `openai`, `anthropic`, `vector-db`, `local-llm` |
| `integration` | Indigo | External service integrations | `stripe`, `auth0`, `sendgrid`, `twilio` |
| `custom` | Gray | User-defined components | Any custom type |

### 3.2 Port Schema

Ports define the typed inputs and outputs of a node, enabling strict I/O contracts.

```yaml
- id: "sql-out"                       # Required. Unique within the node.
  label: "SQL Queries"                # Required. Display label.
  direction: "output"                 # Required. "input" or "output".
  dataType: "sql"                     # Required. Data type identifier.
  required: true                      # Optional. Default: false.
  description: "Outgoing SQL queries" # Optional. Description text.
```

#### Port Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `id` | `string` | ✅ | Unique port ID within the node |
| `label` | `string` | ✅ | Display label |
| `direction` | `enum` | ✅ | `input` or `output` |
| `dataType` | `enum` | ✅ | See Data Types table |
| `required` | `boolean` | ❌ | Whether connection is required (default: `false`) |
| `description` | `string` | ❌ | Description of the port |

#### Data Types

| Type | Description | Typical Usage |
|------|-------------|---------------|
| `string` | Plain text data | Config values, messages |
| `number` | Numeric data | Metrics, counters |
| `boolean` | True/false | Feature flags |
| `object` | Structured object | DTOs, models |
| `array` | Ordered collection | Lists, batches |
| `json` | JSON payload | API request/response bodies |
| `http` | HTTP request/response | REST API communication |
| `websocket` | WebSocket stream | Real-time bidirectional |
| `sql` | SQL queries/results | Database communication |
| `graphql` | GraphQL queries | GraphQL API |
| `stream` | Data stream | Event streams, logs |
| `binary` | Binary data | File uploads, images |

### 3.3 Edge Schema

Edges define connections between node ports.

```yaml
edges:
  - id: "e1"                          # Required. Unique edge identifier.
    sourceNodeId: "frontend"           # Required. Source node ID.
    sourcePortId: "http-out"           # Required. Source port ID.
    targetNodeId: "api"                # Required. Target node ID.
    targetPortId: "http-in"            # Required. Target port ID.
    dataType: "http"                   # Required. Data type flowing through.
    edgeType: "dataflow"               # Required. "dataflow" or "dependency".
    label: "REST API Calls"            # Optional. Display label.
```

#### Edge Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `id` | `string` | ✅ | Unique edge ID |
| `sourceNodeId` | `string` | ✅ | Source node reference |
| `sourcePortId` | `string` | ✅ | Source port reference |
| `targetNodeId` | `string` | ✅ | Target node reference |
| `targetPortId` | `string` | ✅ | Target port reference |
| `dataType` | `enum` | ✅ | Data type (must match port types) |
| `edgeType` | `enum` | ✅ | `dataflow` (data passes) or `dependency` (depends on) |
| `label` | `string` | ❌ | Display label for the edge |

### 3.4 Edge Type Semantics

| Type | Visual | Meaning |
|------|--------|---------|
| `dataflow` | Solid animated line | Data actively flows between components |
| `dependency` | Dashed line | Component depends on another (no active data flow) |

---

## Validation Rules

1. **Unique IDs**: All node IDs must be unique. All edge IDs must be unique. Port IDs must be unique within their parent node.
2. **Valid References**: Edge `sourceNodeId` and `targetNodeId` must reference existing nodes. Edge `sourcePortId` and `targetPortId` must reference existing ports on their respective nodes.
3. **Direction Match**: An edge must connect an `output` port to an `input` port.
4. **Type Compatibility**: The edge `dataType` must be compatible with both the source port's `dataType` and the target port's `dataType`.
5. **No Self-Loops**: A node cannot connect to itself.
6. **Required Ports**: All ports marked `required: true` should have at least one connection.

---

## Example

```markdown
---
name: "Simple Blog"
author: "cortikal"
version: "1.0.0"
tags: [blog, react, nodejs, mongodb]
complexity: "simple"
description: "A simple blog application with React frontend and Node.js API."
---

# Simple Blog Architecture

A minimal blog application with server-side rendering.

​```arch
nodes:
  - id: blog-frontend
    type: react-app
    category: frontend
    label: "Blog Frontend (Next.js)"
    position: { x: 100, y: 200 }
    outputs:
      - id: api-calls
        label: "API Requests"
        direction: output
        dataType: http
        required: true

  - id: blog-api
    type: nodejs-api
    category: backend
    label: "Blog API (Express)"
    position: { x: 400, y: 200 }
    inputs:
      - id: http-in
        label: "HTTP Requests"
        direction: input
        dataType: http
        required: true
    outputs:
      - id: db-queries
        label: "Database Queries"
        direction: output
        dataType: json
        required: true

  - id: blog-db
    type: mongodb
    category: database
    label: "Blog Database (MongoDB)"
    position: { x: 700, y: 200 }
    inputs:
      - id: queries
        label: "Queries"
        direction: input
        dataType: json
        required: true

edges:
  - id: e1
    sourceNodeId: blog-frontend
    sourcePortId: api-calls
    targetNodeId: blog-api
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "REST API"

  - id: e2
    sourceNodeId: blog-api
    sourcePortId: db-queries
    targetNodeId: blog-db
    targetPortId: queries
    dataType: json
    edgeType: dataflow
    label: "CRUD Operations"
​```
```

---

## File Naming Convention

- Architecture files: `arch.md` (one per project/template directory)
- In a project root: `./arch.md`
- In the registry: `registry/templates/<slug>/arch.md`
