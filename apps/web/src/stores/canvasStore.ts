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

// ============================================================
// Store
// ============================================================

interface CanvasState {
  // Graph data
  nodes: ArchNode[];
  edges: ArchEdge[];

  // Selection
  selectedNodeId: string | null;
  selectedEdgeId: string | null;

  // UI State
  isPaletteOpen: boolean;
  isMinimapVisible: boolean;

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
  loadGraph: (nodes: ArchNode[], edges: ArchEdge[]) => void;
  clearGraph: () => void;
}

let nodeIdCounter = 0;
let edgeIdCounter = 0;

const generateNodeId = () => `node-${++nodeIdCounter}`;
const generateEdgeId = () => `edge-${++edgeIdCounter}`;

export const useCanvasStore = create<CanvasState>((set, get) => ({
  nodes: [],
  edges: [],
  selectedNodeId: null,
  selectedEdgeId: null,
  isPaletteOpen: true,
  isMinimapVisible: true,

  onNodesChange: (changes) => {
    set({ nodes: applyNodeChanges(changes, get().nodes) });
  },

  onEdgesChange: (changes) => {
    set({ edges: applyEdgeChanges(changes, get().edges) });
  },

  onConnect: (connection: Connection) => {
    const newEdge: ArchEdge = {
      ...connection,
      id: generateEdgeId(),
      type: "archEdge",
      animated: true,
      data: {
        dataType: "json",
        edgeType: "dataflow",
      },
    };
    set({ edges: addEdge(newEdge, get().edges) });
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

  loadGraph: (nodes, edges) => {
    // Reset counters
    nodeIdCounter = nodes.length;
    edgeIdCounter = edges.length;
    set({ nodes, edges, selectedNodeId: null, selectedEdgeId: null });
  },

  clearGraph: () => {
    nodeIdCounter = 0;
    edgeIdCounter = 0;
    set({ nodes: [], edges: [], selectedNodeId: null, selectedEdgeId: null });
  },
}));
