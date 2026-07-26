---
name: arch-parser-parity
description: Use whenever changing the arch.md format itself, the JSON schema, or either arch parser implementation (packages/arch-parser in TypeScript, Cortikal.ArchParser in C#). Keeps the two parsers, the JSON schema, and the spec doc from drifting out of sync.
---

# Keeping the two arch.md parsers in sync

Cortikal maintains **two independent implementations** of the same `arch.md` format:

- `packages/arch-parser/src/` (TypeScript) — `parser.ts`, `validator.ts`, `types.ts`, `index.ts`. Used by the Next.js frontend.
- `server/src/Cortikal.ArchParser/` (C#) — `ArchMarkdownParser.cs`, `ArchSerializer.cs`, `Validation/`. Used by the .NET backend (`Cortikal.Api`'s `CanvasController`, and by `ArchitectureGeneratorService` in `Cortikal.Infrastructure`).

There is also a third source of truth that must agree with both: `registry/schema/arch-schema.json`, plus the human-readable spec at `docs/architecture/arch-md-spec.md`.

**These four things must always describe the exact same format.** This is the single easiest place for silent drift in this codebase — a change in one and not the others won't fail loudly, it'll just mean the frontend and backend disagree about what's valid.

## When you change any ONE of these, update ALL of these

1. `docs/architecture/arch-md-spec.md` — the prose spec and field tables.
2. `registry/schema/arch-schema.json` — the JSON Schema `definitions` (`Node`, `Port`, `Edge`, `Position`), including any new/changed `enum` lists.
3. `packages/arch-parser/src/types.ts` and `validator.ts` (and `parser.ts` if parsing logic changes).
4. `server/src/Cortikal.ArchParser/ArchMarkdownParser.cs`, `ArchSerializer.cs`, and `Validation/` (C# model types typically live in `Cortikal.Core/Models`).
5. Corresponding tests in `server/tests/Cortikal.ArchParser.Tests/` (`ParserTests.cs`, `SerializerTests.cs`, `ValidationTests.cs` + `Fixtures/`) — add or update fixture `.arch.md` files there. Mirror equivalent coverage in the TS package if it has its own tests.
6. Any registry template under `registry/templates/*/arch.md` that uses a field/enum you renamed or removed.

## Specific parity traps to watch for

- **Enum lists must match exactly** across `arch-schema.json`, the C# `Enums`/model validation, and the TS `validator.ts`. E.g. `category` must stay `frontend | backend | database | infrastructure | ai | integration | custom` everywhere — don't let one side abbreviate (`infra`) while the other doesn't.
- **Required vs optional fields** must match: a field required in the JSON Schema but optional in one parser (or vice versa) means one implementation will accept documents the other rejects.
- **Error message conventions**: existing C# tests assert on substrings like `e.Contains("frontmatter")` / `e.Contains("arch")` / `e.Contains("empty")` for invalid-input cases (see `ParserTests.cs`). If you rework error handling, keep these recognizable substrings (or update the TS validator's error strings to match the same recognizable keywords) so both sides fail with equivalently diagnosable messages.
- **Validation rules** (unique IDs, valid node/port references, direction match, dataType compatibility, no self-loops, required-port connectivity — listed in the spec's "Validation Rules" section) must be enforced identically by both parsers, not just by one.
- **Round-tripping**: `ArchSerializer.cs` (C#) serializes a graph back to markdown; if `packages/arch-parser` also supports serialization, a parsed-then-serialized-then-reparsed document should be equivalent on both sides.

## Process

1. Before changing the format, read `docs/architecture/arch-md-spec.md` and `registry/schema/arch-schema.json` fully so you understand the current contract.
2. Make the spec/schema change first — treat it as the source of truth — then propagate to both parser implementations.
3. Add/update fixtures and tests in `Cortikal.ArchParser.Tests` for the new/changed behavior; add equivalent TS tests if a test setup exists for `packages/arch-parser`.
4. Grep the repo for the old field/enum name across `registry/templates/`, both parser packages, and `packages/shared-types` before considering the change complete, to catch stragglers.
