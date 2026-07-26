import { create } from "zustand";
import {
  type Node,
  type Edge,
  type OnNodesChange,
  type OnEdgesChange,
  type OnConnect,
  type Connection,
  applyNodeChanges,
  applyEdgeChanges,
  addEdge,
  type XYPosition,
} from "@xyflow/react";

// ============================================================
// Types
// ============================================================

export interface PortData {
  id: string;
  label: string;
  direction: "input" | "output";
  dataType: string;
  required: boolean;
  description?: string;
}

export interface ArchNodeData extends Record<string, unknown> {
  label: string;
  type: string;
  category: NodeCategory;
  config: Record<string, unknown>;
  inputs: PortData[];
  outputs: PortData[];
}

export type NodeCategory =
  | "frontend"
  | "backend"
  | "database"
  | "infrastructure"
  | "ai"
  | "integration"
  | "custom";

export type ArchNode = Node<ArchNodeData, "archNode">;

export interface ArchEdgeData extends Record<string, unknown> {
  dataType: string;
  edgeType: "dataflow" | "dependency";
  label?: string;
}

export type ArchEdge = Edge<ArchEdgeData>;

export interface ArchMetadata {
  name: string;
  author: string;
  version: string;
  tags: string[];
  complexity: string;
  description: string;
  createdAt?: string;
  updatedAt?: string;
}

// ============================================================
// Helpers
// ============================================================

/** UUID-based ID generation — collision-safe with imported IDs */
const generateNodeId = () => `node-${crypto.randomUUID().slice(0, 8)}`;
const generateEdgeId = () => `edge-${crypto.randomUUID().slice(0, 8)}`;

/**
 * Port data-type compatibility check.
 * Mirrors the C# ArchValidator.AreTypesCompatible logic.
 */
const JSON_COMPATIBLE = new Set(["json", "object", "array", "string"]);

function arePortTypesCompatible(
  sourceType: string,
  targetType: string
): boolean {
  if (sourceType === targetType) return true;
  if (JSON_COMPATIBLE.has(sourceType) && JSON_COMPATIBLE.has(targetType))
    return true;
  return false;
}

// ============================================================
// Store
// ============================================================

interface CanvasState {
  // Graph data
  nodes: ArchNode[];
  edges: ArchEdge[];

  // Document metadata (preserved across import/export)
  metadata: ArchMetadata | null;

  // Selection
  selectedNodeId: string | null;
  selectedEdgeId: string | null;

  // UI State
  isPaletteOpen: boolean;
  isMinimapVisible: boolean;

  // Connection error feedback
  lastConnectionError: string | null;

  // React Flow callbacks
  onNodesChange: OnNodesChange<ArchNode>;
  onEdgesChange: OnEdgesChange<ArchEdge>;
  onConnect: OnConnect;

  // Node actions
  addNode: (
    type: string,
    category: NodeCategory,
    label: string,
    position: XYPosition,
    inputs?: PortData[],
    outputs?: PortData[]
  ) => void;
  removeNode: (id: string) => void;
  updateNodeData: (id: string, data: Partial<ArchNodeData>) => void;

  // Edge actions
  removeEdge: (id: string) => void;

  // Selection
  setSelectedNode: (id: string | null) => void;
  setSelectedEdge: (id: string | null) => void;

  // UI
  togglePalette: () => void;
  toggleMinimap: () => void;

  // Import/Export
  loadGraph: (
    nodes: ArchNode[],
    edges: ArchEdge[],
    metadata?: ArchMetadata | null
  ) => void;
  clearGraph: () => void;
}

export const useCanvasStore = create<CanvasState>((set, get) => ({
  nodes: [],
  edges: [],
  metadata: null,
  selectedNodeId: null,
  selectedEdgeId: null,
  isPaletteOpen: true,
  isMinimapVisible: true,
  lastConnectionError: null,

  onNodesChange: (changes) => {
    set({ nodes: applyNodeChanges(changes, get().nodes) });
  },

  onEdgesChange: (changes) => {
    set({ edges: applyEdgeChanges(changes, get().edges) });
  },

  onConnect: (connection: Connection) => {
    const { nodes } = get();

    // Look up the source and target nodes/ports
    const sourceNode = nodes.find((n) => n.id === connection.source);
    const targetNode = nodes.find((n) => n.id === connection.target);

    if (!sourceNode || !targetNode) return;

    if (sourceNode.id === targetNode.id) {
      set({ lastConnectionError: "Cannot connect a node to itself (no self-loops)" });
      return;
    }

    const sourcePort = sourceNode.data.outputs.find(
      (p) => p.id === connection.sourceHandle
    );
    const targetPort = targetNode.data.inputs.find(
      (p) => p.id === connection.targetHandle
    );

    // Determine data types — fall back to "json" if ports aren't found
    const sourceType = sourcePort?.dataType ?? "json";
    const targetType = targetPort?.dataType ?? "json";

    // Enforce type compatibility
    if (sourcePort && targetPort && !arePortTypesCompatible(sourceType, targetType)) {
      set({
        lastConnectionError:
          `Type mismatch: ${sourcePort.id} (${sourceType}) → ${targetPort.id} (${targetType})`,
      });
      return; // Reject the connection
    }

    const newEdge: ArchEdge = {
      ...connection,
      id: generateEdgeId(),
      type: "archEdge",
      animated: true,
      data: {
        dataType: sourceType,
        edgeType: "dataflow",
      },
    };
    set({ edges: addEdge(newEdge, get().edges), lastConnectionError: null });
  },

  addNode: (type, category, label, position, inputs = [], outputs = []) => {
    const id = generateNodeId();
    const newNode: ArchNode = {
      id,
      type: "archNode",
      position,
      data: {
        label,
        type,
        category,
        config: {},
        inputs,
        outputs,
      },
    };
    set({ nodes: [...get().nodes, newNode] });
  },

  removeNode: (id) => {
    set({
      nodes: get().nodes.filter((n) => n.id !== id),
      edges: get().edges.filter(
        (e) => e.source !== id && e.target !== id
      ),
      selectedNodeId:
        get().selectedNodeId === id ? null : get().selectedNodeId,
    });
  },

  updateNodeData: (id, data) => {
    set({
      nodes: get().nodes.map((n) =>
        n.id === id ? { ...n, data: { ...n.data, ...data } } : n
      ),
    });
  },

  removeEdge: (id) => {
    set({
      edges: get().edges.filter((e) => e.id !== id),
      selectedEdgeId:
        get().selectedEdgeId === id ? null : get().selectedEdgeId,
    });
  },

  setSelectedNode: (id) =>
    set({ selectedNodeId: id, selectedEdgeId: null }),

  setSelectedEdge: (id) =>
    set({ selectedEdgeId: id, selectedNodeId: null }),

  togglePalette: () =>
    set({ isPaletteOpen: !get().isPaletteOpen }),

  toggleMinimap: () =>
    set({ isMinimapVisible: !get().isMinimapVisible }),

  loadGraph: (nodes, edges, metadata = null) => {
    set({
      nodes,
      edges,
      metadata: metadata ?? get().metadata,
      selectedNodeId: null,
      selectedEdgeId: null,
    });
  },

  clearGraph: () => {
    set({
      nodes: [],
      edges: [],
      metadata: null,
      selectedNodeId: null,
      selectedEdgeId: null,
    });
  },
}));

