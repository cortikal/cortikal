/**
 * Cortikal — arch.md TypeScript Parser
 *
 * Parses arch.md files into structured ArchDocument objects.
 * Handles YAML frontmatter extraction and ```arch graph block parsing.
 */

export { parseArchMarkdown } from "./parser";
export { validateArchDocument } from "./validator";
export type { ParseResult, ValidationResult } from "./types";
