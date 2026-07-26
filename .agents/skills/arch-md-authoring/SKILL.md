---
name: arch-md-authoring
description: Use whenever writing, editing, or generating an arch.md file — including registry templates, AI-generated architectures in Cortikal.Infrastructure/ArchitectureGeneratorService, or example docs. Ensures the node/edge/port graph is valid against the Cortikal arch.md spec and schema.
---

# Authoring valid arch.md files

`arch.md` is Cortikal's core data format: a markdown file with YAML frontmatter, optional markdown description, and a fenced ` ```arch ` code block containing a YAML `nodes`/`edges` graph. The authoritative definitions are:

- Spec: `docs/architecture/arch-md-spec.md`
- JSON Schema for the graph block: `registry/schema/arch-schema.json`
- Reference examples: `registry/templates/*/arch.md`

Always check these three sources before inventing a field, enum value, or shape — do not guess.

## Required structure

```markdown
---
name: "..."          # required
author: "..."        # required
version: "1.0.0"     # required, semver
tags: [..]            # required, string array
complexity: "simple" # required: simple | moderate | complex | enterprise
description: "..."   # required, 1-2 sentences
createdAt: "..."      # optional, ISO 8601
updatedAt: "..."      # optional, ISO 8601
---

<optional free-form markdown description>

​```arch
nodes:
  - id: kebab-case-id      # required, pattern ^[a-z0-9][a-z0-9-]*$
    type: "..."            # required, e.g. react-app, dotnet-api, postgresql
    category: "..."        # required enum, see below
    label: "..."           # required, human-readable
    position: { x: 0, y: 0 } # required
    config: { }             # optional, type-specific key-values
    inputs: [ ... ports ]    # optional
    outputs: [ ... ports ]   # optional
edges:
  - id: "..."             # required, unique
    sourceNodeId: "..."   # required
    sourcePortId: "..."   # required
    targetNodeId: "..."   # required
    targetPortId: "..."   # required
    dataType: "..."       # required, must match both ports' dataType
    edgeType: "dataflow"  # required: dataflow | dependency
    label: "..."           # optional
​```
```

## Enums you must not invent values outside of

- `category` (Node): `frontend`, `backend`, `database`, `infrastructure`, `ai`, `integration`, `custom`
- `complexity` (frontmatter): `simple`, `moderate`, `complex`, `enterprise`
- Port/edge `dataType`: `string`, `number`, `boolean`, `object`, `array`, `json`, `http`, `websocket`, `sql`, `graphql`, `stream`, `binary`
- Port `direction`: `input`, `output`
- `edgeType`: `dataflow` (solid animated — active data flow) or `dependency` (dashed — no active data flow)

Note the schema (`registry/schema/arch-schema.json`) uses `"infrastructure"` for the node category, while the spec table header elsewhere abbreviates it as `infra` in CSS class names (`.node--infra`) — the **YAML value must be `infrastructure`**, matching the schema. Don't write `infra` in an `arch.md` file.

## Validation rules to self-check before finishing

1. Every node `id` is unique and matches `^[a-z0-9][a-z0-9-]*$` (kebab-case).
2. Every edge `id` is unique; port `id`s are unique within their own node.
3. Every edge's `sourceNodeId`/`targetNodeId` reference nodes that actually exist in `nodes`, and `sourcePortId`/`targetPortId` reference ports that actually exist on those nodes.
4. Every edge connects an `output` port (source) to an `input` port (target) — never input-to-input or output-to-output.
5. The edge's `dataType` matches both the source and target port's `dataType`.
6. No node connects to itself.
7. Ports marked `required: true` should have at least one edge connected to them in a "complete" example — flag it if not, rather than silently leaving it dangling.
8. Node count roughly matches the declared `complexity` (simple: 1–4, moderate: 4–8, complex: 8–15, enterprise: 15+) — mismatches aren't fatal but are worth calling out.

## Where this format is consumed

- `packages/arch-parser` (TypeScript) and `Cortikal.ArchParser` (C#) both parse this exact format — see the `arch-parser-parity` skill if you're changing the format itself, not just authoring a document.
- `registry/schema/arch-schema.json` is used for schema validation of the graph block.
- `Cortikal.Infrastructure`'s `ArchitectureGeneratorService` produces raw YAML from an LLM and wraps it into a full `arch.md` document before parsing — if you touch that service or its prompt, make sure the LLM is steered to only emit values from the enums above.

## When authoring a new registry template

Also see the `arch-template-contribution` skill for directory/README conventions beyond the `arch.md` content itself.
