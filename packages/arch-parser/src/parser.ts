/**
 * Cortikal — arch.md Parser Implementation
 *
 * Parses arch.md markdown files into ArchDocument objects.
 * Format:
 *   ---
 *   name: My Architecture
 *   author: ...
 *   ---
 *   # Description (optional markdown)
 *   ```arch
 *   nodes: ...
 *   edges: ...
 *   ```
 */

import { parse as parseYaml } from "yaml";
import type { ArchDocument, ArchMetadata, ArchGraph } from "@cortikal/shared-types";
import type { ParseResult, ParseError } from "./types";

const FRONTMATTER_REGEX = /^---\r?\n([\s\S]*?)\r?\n---/;
const ARCH_BLOCK_REGEX = /```arch\r?\n([\s\S]*?)\r?\n```/;

/**
 * Parse an arch.md file content string into an ArchDocument.
 */
export function parseArchMarkdown(content: string): ParseResult {
  const errors: ParseError[] = [];

  // 1. Extract YAML frontmatter
  const frontmatterMatch = content.match(FRONTMATTER_REGEX);
  if (!frontmatterMatch) {
    errors.push({
      message: "Missing YAML frontmatter (expected --- delimited block at start of file)",
      line: 1,
      severity: "error",
    });
    return { success: false, errors };
  }

  let metadata: ArchMetadata;
  try {
    const parsed = parseYaml(frontmatterMatch[1]);
    metadata = {
      name: parsed.name ?? "Untitled",
      author: parsed.author ?? "Unknown",
      version: parsed.version ?? "0.1.0",
      tags: Array.isArray(parsed.tags) ? parsed.tags : [],
      complexity: parsed.complexity ?? "simple",
      description: parsed.description ?? "",
      createdAt: parsed.createdAt,
      updatedAt: parsed.updatedAt,
    };
  } catch (e) {
    errors.push({
      message: `Failed to parse YAML frontmatter: ${e instanceof Error ? e.message : String(e)}`,
      line: 1,
      severity: "error",
    });
    return { success: false, errors };
  }

  // 2. Extract description (markdown between frontmatter and arch block)
  const afterFrontmatter = content.slice(frontmatterMatch[0].length);
  const archBlockMatch = afterFrontmatter.match(ARCH_BLOCK_REGEX);

  let description: string | undefined;
  if (archBlockMatch) {
    const beforeArchBlock = afterFrontmatter.slice(0, archBlockMatch.index).trim();
    if (beforeArchBlock) {
      description = beforeArchBlock;
    }
  }

  // 3. Extract and parse ```arch block
  if (!archBlockMatch) {
    errors.push({
      message: "Missing ```arch code block",
      severity: "error",
    });
    return { success: false, errors };
  }

  let graph: ArchGraph;
  try {
    const graphData = parseYaml(archBlockMatch[1]);
    graph = {
      nodes: Array.isArray(graphData?.nodes) ? graphData.nodes : [],
      edges: Array.isArray(graphData?.edges) ? graphData.edges : [],
    };
  } catch (e) {
    errors.push({
      message: `Failed to parse arch block YAML: ${e instanceof Error ? e.message : String(e)}`,
      severity: "error",
    });
    return { success: false, errors };
  }

  const document: ArchDocument = {
    metadata,
    graph,
    description,
  };

  return { success: true, document, errors };
}
