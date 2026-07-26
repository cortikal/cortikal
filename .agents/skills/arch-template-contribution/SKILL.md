---
name: arch-template-contribution
description: Use whenever adding, editing, or reviewing a community architecture template under registry/templates. Ensures new templates follow the registry's directory structure and are listed correctly.
---

# Adding a template to the registry

`registry/` holds community-contributed architecture templates, each in its own directory under `registry/templates/`. Existing templates: `web-app-basic`, `realtime-chat`, `saas-starter`, `ecommerce` — check one of these (`web-app-basic` is the simplest) as a reference before adding a new one.

## Required structure for a new template

Per `registry/README.md`, a new template directory (`registry/templates/<descriptive-slug>/`) must contain:

1. **`arch.md`** — the architecture definition itself. Must be valid per the `arch-md-authoring` skill (correct frontmatter, valid enum values, valid node/edge/port references). The `complexity` field in its frontmatter should be honest relative to actual node count (simple: 1–4, moderate: 4–8, complex: 8–15, enterprise: 15+).
2. **`README.md`** — human-readable description of the architecture: what it's for, what stack it uses, and why someone would pick this template. This is separate from the `description` field inside `arch.md`'s frontmatter (that one is short/machine-facing; the template `README.md` can be longer and more explanatory).
3. **`preview.png`** (optional) — a screenshot of the rendered canvas, if available.

Use a **descriptive kebab-case slug** for the directory name (matching the pattern of existing ones: `web-app-basic`, `realtime-chat`, `saas-starter`, `ecommerce`) — not a generic name like `template1` or the author's username.

## Registering the template

After adding the directory and files, update the table in `registry/README.md`'s "Available Templates" section:

```markdown
| [your-slug](templates/your-slug/) | <Complexity> | <One-line description> |
```

Match the existing row format exactly (link text = slug, complexity = title-case word matching the `arch.md` complexity value, description = short one-liner). Templates are surfaced through `Cortikal.Api`'s `RegistryController` and fetched by the frontend via `ApiClient.getTemplates()`/`getTemplateContent()` (`apps/web/src/lib/api.ts`) — if the registry-loading logic on either side assumes a fixed file layout (e.g. always looking for `arch.md` at a specific relative path), keep new templates conforming to that same layout rather than introducing variations.

## Process

1. Check `registry/templates/web-app-basic/` as the reference structure.
2. Write `arch.md` following the `arch-md-authoring` skill.
3. Write `README.md` describing the architecture in more depth than the frontmatter `description`.
4. Add the row to `registry/README.md`'s template table.
5. If you're contributing via PR (not editing directly), follow the commit/PR conventions in `docs/guides/contributing.md`.
