/**
 * Cortikal — Graph Type Definitions
 *
 * Defines the graph structure that represents an architecture
 * on the visual canvas and in arch.md files.
 */

import type { Port, PortDataType, NodeCategory } from "./node-types";

/** A positioned node instance on the canvas */
export interface GraphNode {
  /** Unique node instance ID */
  id: string;
  /** Reference to the NodeDefinition type */
  type: string;
  /** Category of this node */
  category: NodeCategory;
  /** Display label (user-editable) */
  label: string;
  /** Canvas position */
  position: {
    x: number;
    y: number;
  };
  /** Instance-specific configuration */
  config: Record<string, unknown>;
  /** Resolved input ports */
  inputs: Port[];
  /** Resolved output ports */
  outputs: Port[];
}

/** A connection between two node ports */
export interface GraphEdge {
  /** Unique edge ID */
  id: string;
  /** Source node ID */
  sourceNodeId: string;
  /** Source port ID */
  sourcePortId: string;
  /** Target node ID */
  targetNodeId: string;
  /** Target port ID */
  targetPortId: string;
  /** Data type flowing through this edge */
  dataType: PortDataType;
  /** Whether this is a data flow or dependency relationship */
  edgeType: "dataflow" | "dependency";
  /** Optional label for the edge */
  label?: string;
}

/** Complete graph representing an architecture */
export interface ArchGraph {
  /** All nodes in the graph */
  nodes: GraphNode[];
  /** All edges connecting nodes */
  edges: GraphEdge[];
}

/** YAML frontmatter metadata for arch.md */
export interface ArchMetadata {
  /** Architecture name */
  name: string;
  /** Author or organization */
  author: string;
  /** Version string */
  version: string;
  /** Descriptive tags for search/filtering */
  tags: string[];
  /** Complexity rating: simple, moderate, complex, enterprise */
  complexity: "simple" | "moderate" | "complex" | "enterprise";
  /** Brief description */
  description: string;
  /** Creation timestamp (ISO 8601) */
  createdAt?: string;
  /** Last modified timestamp (ISO 8601) */
  updatedAt?: string;
}

/** Complete arch.md document model */
export interface ArchDocument {
  /** YAML frontmatter metadata */
  metadata: ArchMetadata;
  /** The architecture graph */
  graph: ArchGraph;
  /** Optional markdown description between frontmatter and graph */
  description?: string;
}
