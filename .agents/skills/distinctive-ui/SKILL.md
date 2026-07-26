---
name: distinctive-ui
description: Use whenever creating or editing UI in this repo (apps/web, packages/ui-components, any *.css/*.tsx touching layout, color, typography, motion, or component visuals). Steers UI work away from the generic "AI-generated SaaS" look and toward a distinctive visual identity for Cortikal.
---

# Distinctive UI for Cortikal

Cortikal's current UI foundation (`apps/web/src/styles/tokens.css`, `animations.css`, `nodes.css`) uses a look that is now extremely common in AI-generated products: dark `#0a0a0f` background, indigo/violet gradient accent (`#6366f1` → `#8b5cf6`), glassmorphism cards (`backdrop-filter: blur()` + translucent white overlays), "Inter" for everything, glow-pulse shadows, and `fadeInUp`/stagger entrance animations. This combination is the default output of most AI page builders (v0, Bolt, Lovable, etc.), so products that use it blend together and look "AI-made."

Your job when touching UI: actively steer away from this default aesthetic and toward something that feels deliberately designed for Cortikal specifically, not generated.

## Tells to avoid (the "generic AI UI" checklist)

Treat any of these as a signal to reconsider, unless there's a specific, stated reason to keep it:

1. **Indigo/violet/purple as the primary accent** (`#6366f1`, `#8b5cf6`, `#7c3aed`, or near variants) on a near-black background. This is the single most recognizable AI-generated tell.
2. **Glassmorphism as the default surface treatment** — translucent `rgba(255,255,255,0.03–0.1)` panels with `backdrop-filter: blur()` on every card/panel/node.
3. **"Inter" (or "Inter" + a generic mono) as the only typeface.** It's the default AI-builder font pairing.
4. **Glow-pulse / soft box-shadow halos** around focused or selected elements as the primary way to show emphasis.
5. **Generic entrance choreography**: everything fades/slides up 12px and staggers in by ~60ms — used almost everywhere AI-generated marketing sites appear.
6. **Rounded-2xl soft cards in a symmetric grid** for feature/landing sections, often paired with a small pill/badge ("New", "Beta") and a two-tone gradient headline.
7. **Bento-grid layouts** as a default way to show "features."
8. Overuse of `border-radius` in the 12–24px range everywhere, making every surface look interchangeable.

None of these are wrong in isolation — they're wrong because they're the *default, unexamined* choice. If you use one, it should be because it's the right call for Cortikal specifically, not because it's what an AI would generate first.

## What to do instead

- **Anchor decisions in Cortikal's actual identity**: it's a precision tool for architects/engineers to design system graphs and watch agents build them — not a generic marketing SaaS. Lean into an "engineering instrument" feel (think: schematic/blueprint precision, circuit-board and neural-signal motifs tied to the "cortex" concept) rather than a "landing page" feel. Every visual choice should be traceable to something specific about what Cortikal *is* or *does* (nodes, edges, typed ports, agents, data flow), not a generic app aesthetic.
- **Commit to a specific, non-default palette** and use it consistently instead of introducing indigo/violet by default. If you need a new accent, pick something deliberately, name it in `tokens.css` with a comment explaining the reasoning, and check it against the existing `--color-node-*` category colors so it doesn't collide.
- **Pick a distinctive type pairing** — e.g. a technical/geometric or slightly unusual display face for headings paired with a plain, highly-legible body face (and keep `--font-mono` for code/ports/technical labels, which already fits the tool's nature). Avoid defaulting back to Inter-only.
- **Make motion purposeful, not decorative.** Prefer animations that reflect the product's real mechanics — e.g. the existing `dataFlow` (dashed-line flow along edges) and `nodeEnter` in `animations.css` are good examples because they're specific to the node-graph metaphor. Be wary of adding more generic `fadeInUp`/stagger/shimmer effects just to make things feel "premium" — that's exactly the generic tell.
- **Vary surface treatment deliberately.** Not every panel needs blur + translucency. Consider solid elevated surfaces, hairline borders, or flat panels with a single sharp accent (e.g. the existing `.node::before` category accent strip in `nodes.css` is a good non-generic pattern — a colored top edge instead of a full glow) and reuse that kind of asymmetric, specific detail over blanket glow effects.
- **Design one or two signature interactions** unique to Cortikal (e.g. a distinctive way ports connect/snap, a distinctive way an agent "claims" a node while working on it in the Swarm view, a distinctive Mission Control log/terminal treatment) rather than relying on generic micro-interactions everywhere else.
- **Prefer asymmetry and intentional layout over safe centered/grid defaults** for marketing-style pages (landing/Spark phase), since symmetric card grids are the fastest way to look AI-generated.

## Process when doing UI work

1. Before adding new colors/animations/components, check `apps/web/src/styles/tokens.css`, `animations.css`, `nodes.css`, and `packages/ui-components/src` for existing patterns. Reuse and extend deliberately rather than bolting on new generic patterns.
2. After drafting a UI change, run it against the "tells to avoid" checklist above. If it matches multiple items with no specific justification, revise it.
3. If you're about to introduce a new color, animation, or layout pattern, ask: "Would this be indistinguishable from a v0/Lovable/Bolt-generated screen?" If yes, make it more specific to Cortikal before finishing.
4. When in doubt about a direction (palette, type pairing, a new motif), briefly state the two options and your reasoning to the user rather than silently defaulting to the safest/most common choice.
5. Keep accessibility and legibility intact while doing this — distinctive should never mean lower contrast or harder-to-read text. Check contrast against `--color-bg-*`/`--color-text-*` pairs when introducing new colors.
