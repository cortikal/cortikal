---
name: changelog-discipline
description: Use whenever completing a feature-sized change (new endpoint, component, agent, scanner, parser change, etc.) in this repo. Ensures CHANGELOG.md stays accurate and follows the project's existing Keep a Changelog + phase-based structure.
---

# Updating CHANGELOG.md

`CHANGELOG.md` follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and [Semantic Versioning](https://semver.org/), under an `## [Unreleased]` heading. This repo additionally organizes `[Unreleased]` into **phase-based subsections** (matching the 4 phases from `README.md`: Spark, Canvas, Swarm, Mission Control), e.g.:

```markdown
## [Unreleased]

### Phase 3 — Prompt-to-Canvas & Canvas Completions

#### Added
- ...

#### Changed
- ...
```

## Rules to follow

1. **Every feature-sized change gets a changelog entry** under `[Unreleased]` — a new endpoint, a new UI component, a new agent role, a new scanner, a parser/schema change, a behavior change a user or contributor would notice. Trivial internal refactors with no observable behavior change don't need one.
2. **Use the correct subsection type**: `Added` (new capability), `Changed` (behavior change to something existing), `Fixed` (bug fix), `Removed`, `Deprecated`, `Security` — matching standard Keep a Changelog categories. The existing file uses `#### Added` and `#### Changed` under phase headers; follow that nesting (phase heading → category sub-heading → bullet list), don't flatten it back to plain `### Added`.
3. **Group under the current phase being worked on.** If the phase heading for the current work already exists in `[Unreleased]`, add your bullets to its existing `Added`/`Changed`/`Fixed` list rather than creating a duplicate phase heading. If it doesn't exist yet, add a new `### Phase N — <name>` heading above (or below, matching existing ordering — newest phase work at the top, per the current file) the older entries.
4. **Write entries as concise, specific, user/contributor-facing statements**, not implementation narration. Compare the existing style:
   - Good: `` `POST /api/canvas/generate` endpoint in `CanvasController` — accepts `{ prompt: string }`, returns `200 ArchParseResult` on success or `422` with error list on failure ``
   - Avoid: "Fixed some bugs in the canvas code" (too vague) or a blow-by-blow of every intermediate commit (too granular).
5. **Reference concrete symbols/files in backticks** (class names, endpoint routes, component names) the way existing entries do — this makes the changelog useful as a map of what changed, not just marketing copy.
6. Leave the release-cut mechanics (moving `[Unreleased]` content under a new version heading with a date, per Semantic Versioning) to whoever actually cuts a release — don't invent a version number yourself when just adding an entry during regular development.

## Process

After finishing a feature-sized change, before considering the task done: open `CHANGELOG.md`, find (or create) the right phase heading under `[Unreleased]`, and add a bullet under the right category (`Added`/`Changed`/`Fixed`/etc.) describing what you just shipped, in the style shown above.
