/**
 * arch.md ↔ React Flow conversion utilities.
 *
 * Converts parsed arch.md graph data into React Flow nodes/edges
 * and vice versa for export.
 */

import type { ArchNode, ArchEdge, ArchNodeData, PortData } from "../stores/canvasStore";

interface ArchMdNode {
  id: string;
  type: string;
  category: string;
  label: string;
  position: { x: number; y: number };
  config?: Record<string, unknown>;
  inputs?: ArchMdPort[];
  outputs?: ArchMdPort[];
}

interface ArchMdPort {
  id: string;
  label: string;
  direction: "input" | "output";
  dataType: string;
  required?: boolean;
  description?: string;
}

interface ArchMdEdge {
  id: string;
  sourceNodeId: string;
  sourcePortId: string;
  targetNodeId: string;
  targetPortId: string;
  dataType: string;
  edgeType: "dataflow" | "dependency";
  label?: string;
}

interface ArchMdGraph {
  nodes: ArchMdNode[];
  edges?: ArchMdEdge[];
}

/**
 * Convert arch.md graph data to React Flow nodes and edges.
 */
export function archToReactFlow(graph: ArchMdGraph): {
  nodes: ArchNode[];
  edges: ArchEdge[];
} {
  const nodes: ArchNode[] = graph.nodes.map((node) => ({
    id: node.id,
    type: "archNode",
    position: { x: node.position.x, y: node.position.y },
    data: {
      label: node.label,
      type: node.type,
      category: node.category as ArchNodeData["category"],
      config: node.config ?? {},
      inputs: (node.inputs ?? []).map(convertPort),
      outputs: (node.outputs ?? []).map(convertPort),
    },
  }));

  const edges: ArchEdge[] = (graph.edges ?? []).map((edge) => ({
    id: edge.id,
    source: edge.sourceNodeId,
    sourceHandle: edge.sourcePortId,
    target: edge.targetNodeId,
    targetHandle: edge.targetPortId,
    type: "archEdge",
    animated: edge.edgeType === "dataflow",
    data: {
      dataType: edge.dataType,
      edgeType: edge.edgeType,
      label: edge.label,
    },
  }));

  return { nodes, edges };
}

/**
 * Convert React Flow nodes and edges back to arch.md graph format.
 */
export function reactFlowToArch(
  nodes: ArchNode[],
  edges: ArchEdge[]
): ArchMdGraph {
  return {
    nodes: nodes.map((node) => ({
      id: node.id,
      type: node.data.type,
      category: node.data.category,
      label: node.data.label,
      position: {
        x: Math.round(node.position.x),
        y: Math.round(node.position.y),
      },
      config:
        Object.keys(node.data.config).length > 0
          ? node.data.config
          : undefined,
      inputs:
        node.data.inputs.length > 0
          ? node.data.inputs.map(exportPort)
          : undefined,
      outputs:
        node.data.outputs.length > 0
          ? node.data.outputs.map(exportPort)
          : undefined,
    })),
    edges: edges.map((edge) => ({
      id: edge.id,
      sourceNodeId: edge.source!,
      sourcePortId: edge.sourceHandle ?? "",
      targetNodeId: edge.target!,
      targetPortId: edge.targetHandle ?? "",
      dataType: edge.data?.dataType ?? "json",
      edgeType: edge.data?.edgeType ?? "dataflow",
      label: edge.data?.label,
    })),
  };
}

function convertPort(port: ArchMdPort): PortData {
  return {
    id: port.id,
    label: port.label,
    direction: port.direction,
    dataType: port.dataType,
    required: port.required ?? false,
    description: port.description,
  };
}

function exportPort(port: PortData): ArchMdPort {
  return {
    id: port.id,
    label: port.label,
    direction: port.direction,
    dataType: port.dataType,
    required: port.required,
    description: port.description,
  };
}

/**
 * Demo data — a sample architecture to pre-load.
 */
export const demoGraph: ArchMdGraph = {
  nodes: [
    {
      id: "frontend",
      type: "react-app",
      category: "frontend",
      label: "React Frontend",
      position: { x: 80, y: 220 },
      outputs: [
        {
          id: "api-out",
          label: "API Requests",
          direction: "output",
          dataType: "http",
          required: true,
        },
      ],
    },
    {
      id: "api",
      type: "dotnet-api",
      category: "backend",
      label: ".NET API",
      position: { x: 420, y: 220 },
      inputs: [
        {
          id: "http-in",
          label: "HTTP Requests",
          direction: "input",
          dataType: "http",
          required: true,
        },
      ],
      outputs: [
        {
          id: "db-out",
          label: "DB Queries",
          direction: "output",
          dataType: "sql",
          required: true,
        },
        {
          id: "cache-out",
          label: "Cache",
          direction: "output",
          dataType: "json",
          required: false,
        },
      ],
    },
    {
      id: "database",
      type: "postgresql",
      category: "database",
      label: "PostgreSQL",
      position: { x: 760, y: 140 },
      inputs: [
        {
          id: "sql-in",
          label: "SQL Queries",
          direction: "input",
          dataType: "sql",
          required: true,
        },
      ],
    },
    {
      id: "cache",
      type: "redis",
      category: "database",
      label: "Redis Cache",
      position: { x: 760, y: 340 },
      inputs: [
        {
          id: "cache-in",
          label: "Cache Ops",
          direction: "input",
          dataType: "json",
          required: true,
        },
      ],
    },
  ],
  edges: [
    {
      id: "e1",
      sourceNodeId: "frontend",
      sourcePortId: "api-out",
      targetNodeId: "api",
      targetPortId: "http-in",
      dataType: "http",
      edgeType: "dataflow",
      label: "REST API",
    },
    {
      id: "e2",
      sourceNodeId: "api",
      sourcePortId: "db-out",
      targetNodeId: "database",
      targetPortId: "sql-in",
      dataType: "sql",
      edgeType: "dataflow",
      label: "Queries",
    },
    {
      id: "e3",
      sourceNodeId: "api",
      sourcePortId: "cache-out",
      targetNodeId: "cache",
      targetPortId: "cache-in",
      dataType: "json",
      edgeType: "dataflow",
      label: "Session Cache",
    },
  ],
};
