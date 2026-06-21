/**
 * Cortikal — ArchDocument Validator
 *
 * Validates structural integrity and I/O contract consistency
 * of parsed ArchDocument objects.
 */

import type { ArchDocument } from "@cortikal/shared-types";
import type { ValidationResult, ValidationError, ValidationWarning } from "./types";

/**
 * Validate an ArchDocument for structural correctness.
 */
export function validateArchDocument(doc: ArchDocument): ValidationResult {
  const errors: ValidationError[] = [];
  const warnings: ValidationWarning[] = [];

  // Validate metadata
  if (!doc.metadata.name || doc.metadata.name.trim() === "") {
    errors.push({
      path: "metadata.name",
      message: "Architecture name is required",
      code: "MISSING_NAME",
    });
  }

  // Validate nodes
  const nodeIds = new Set<string>();
  for (const node of doc.graph.nodes) {
    if (!node.id) {
      errors.push({
        path: `graph.nodes`,
        message: "Node is missing an 'id' field",
        code: "MISSING_NODE_ID",
      });
      continue;
    }

    if (nodeIds.has(node.id)) {
      errors.push({
        path: `graph.nodes.${node.id}`,
        message: `Duplicate node ID: '${node.id}'`,
        code: "DUPLICATE_NODE_ID",
      });
    }
    nodeIds.add(node.id);

    if (!node.type) {
      errors.push({
        path: `graph.nodes.${node.id}.type`,
        message: `Node '${node.id}' is missing a 'type' field`,
        code: "MISSING_NODE_TYPE",
      });
    }

    if (!node.label) {
      warnings.push({
        path: `graph.nodes.${node.id}.label`,
        message: `Node '${node.id}' has no label`,
        code: "MISSING_NODE_LABEL",
      });
    }
  }

  // Validate edges
  const edgeIds = new Set<string>();
  for (const edge of doc.graph.edges) {
    if (!edge.id) {
      errors.push({
        path: "graph.edges",
        message: "Edge is missing an 'id' field",
        code: "MISSING_EDGE_ID",
      });
      continue;
    }

    if (edgeIds.has(edge.id)) {
      errors.push({
        path: `graph.edges.${edge.id}`,
        message: `Duplicate edge ID: '${edge.id}'`,
        code: "DUPLICATE_EDGE_ID",
      });
    }
    edgeIds.add(edge.id);

    if (!nodeIds.has(edge.sourceNodeId)) {
      errors.push({
        path: `graph.edges.${edge.id}.sourceNodeId`,
        message: `Edge '${edge.id}' references non-existent source node '${edge.sourceNodeId}'`,
        code: "INVALID_EDGE_SOURCE",
      });
    }

    if (!nodeIds.has(edge.targetNodeId)) {
      errors.push({
        path: `graph.edges.${edge.id}.targetNodeId`,
        message: `Edge '${edge.id}' references non-existent target node '${edge.targetNodeId}'`,
        code: "INVALID_EDGE_TARGET",
      });
    }
  }

  // Warn on orphan nodes (no edges)
  for (const node of doc.graph.nodes) {
    const hasEdge = doc.graph.edges.some(
      (e) => e.sourceNodeId === node.id || e.targetNodeId === node.id
    );
    if (!hasEdge && doc.graph.nodes.length > 1) {
      warnings.push({
        path: `graph.nodes.${node.id}`,
        message: `Node '${node.id}' has no connections`,
        code: "ORPHAN_NODE",
      });
    }
  }

  return {
    valid: errors.length === 0,
    errors,
    warnings,
  };
}
