/**
 * Cortikal — ArchDocument Validator
 *
 * Validates structural integrity and I/O contract consistency
 * of parsed ArchDocument objects.
 */

import type { ArchDocument } from "@cortikal/shared-types";
import type { ValidationResult, ValidationError, ValidationWarning } from "./types";

/**
 * Check if two data types are compatible.
 */
function arePortTypesCompatible(sourceType: string, targetType: string): boolean {
  if (sourceType === targetType) return true;
  
  const jsonCompatible = new Set(["json", "rest", "graphql", "grpc", "http"]);
  if (jsonCompatible.has(sourceType) && jsonCompatible.has(targetType)) {
    return true;
  }
  
  return false;
}

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

    const sourceNode = doc.graph.nodes.find((n: any) => n.id === edge.sourceNodeId);
    if (!sourceNode) {
      errors.push({
        path: `graph.edges.${edge.id}.sourceNodeId`,
        message: `Edge '${edge.id}' references non-existent source node '${edge.sourceNodeId}'`,
        code: "INVALID_EDGE_SOURCE",
      });
    }

    const targetNode = doc.graph.nodes.find((n: any) => n.id === edge.targetNodeId);
    if (!targetNode) {
      errors.push({
        path: `graph.edges.${edge.id}.targetNodeId`,
        message: `Edge '${edge.id}' references non-existent target node '${edge.targetNodeId}'`,
        code: "INVALID_EDGE_TARGET",
      });
    }

    if (edge.sourceNodeId === edge.targetNodeId) {
      errors.push({
        path: `graph.edges.${edge.id}`,
        message: `Edge '${edge.id}' creates a self-loop on node '${edge.sourceNodeId}'`,
        code: "SELF_LOOP",
      });
    }

    if (sourceNode && targetNode) {
      const sourcePort = sourceNode.outputs.find((p: any) => p.id === edge.sourcePortId);
      if (!sourcePort) {
        errors.push({
          path: `graph.edges.${edge.id}.sourcePortId`,
          message: `Edge '${edge.id}': Source port '${edge.sourcePortId}' does not exist on node '${edge.sourceNodeId}'.`,
          code: "INVALID_EDGE_SOURCE_PORT",
        });
      } else if (sourcePort.direction !== "output") {
        errors.push({
          path: `graph.edges.${edge.id}.sourcePortId`,
          message: `Edge '${edge.id}': Source port '${edge.sourcePortId}' must be an output port.`,
          code: "INVALID_EDGE_SOURCE_DIRECTION",
        });
      }

      const targetPort = targetNode.inputs.find((p: any) => p.id === edge.targetPortId);
      if (!targetPort) {
        errors.push({
          path: `graph.edges.${edge.id}.targetPortId`,
          message: `Edge '${edge.id}': Target port '${edge.targetPortId}' does not exist on node '${edge.targetNodeId}'.`,
          code: "INVALID_EDGE_TARGET_PORT",
        });
      } else if (targetPort.direction !== "input") {
        errors.push({
          path: `graph.edges.${edge.id}.targetPortId`,
          message: `Edge '${edge.id}': Target port '${edge.targetPortId}' must be an input port.`,
          code: "INVALID_EDGE_TARGET_DIRECTION",
        });
      }

      if (sourcePort && targetPort) {
        if (!arePortTypesCompatible(sourcePort.dataType, targetPort.dataType)) {
          errors.push({
            path: `graph.edges.${edge.id}`,
            message: `Edge '${edge.id}': Type mismatch between source '${sourcePort.dataType}' and target '${targetPort.dataType}'.`,
            code: "TYPE_MISMATCH",
          });
        }
      }
    }
  }

  // Warn on orphan nodes (no edges)
  for (const node of doc.graph.nodes) {
    const hasEdge = doc.graph.edges.some(
      (e: any) => e.sourceNodeId === node.id || e.targetNodeId === node.id
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
