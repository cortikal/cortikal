"use client";

import React, { useCallback, useMemo } from "react";
import {
  ReactFlow,
  ReactFlowProvider,
  Background,
  BackgroundVariant,
  MiniMap,
} from "@xyflow/react";
import "@xyflow/react/dist/style.css";

import ArchNodeComponent from "./ArchNode";
import ArchEdgeComponent from "./ArchEdge";
import NodePalette from "./NodePalette";
import CanvasToolbar from "./CanvasToolbar";
import { useCanvasStore } from "../../stores/canvasStore";
import styles from "./CanvasEditor.module.css";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const nodeTypes = { archNode: ArchNodeComponent } as any;
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const edgeTypes = { archEdge: ArchEdgeComponent } as any;

function CanvasEditorInner() {
  const nodes = useCanvasStore((s) => s.nodes);
  const edges = useCanvasStore((s) => s.edges);
  const onNodesChange = useCanvasStore((s) => s.onNodesChange);
  const onEdgesChange = useCanvasStore((s) => s.onEdgesChange);
  const onConnect = useCanvasStore((s) => s.onConnect);
  const setSelectedNode = useCanvasStore((s) => s.setSelectedNode);
  const setSelectedEdge = useCanvasStore((s) => s.setSelectedEdge);
  const isMinimapVisible = useCanvasStore((s) => s.isMinimapVisible);

  const handleNodeClick = useCallback(
    (_: React.MouseEvent, node: { id: string }) => {
      setSelectedNode(node.id);
    },
    [setSelectedNode]
  );

  const handleEdgeClick = useCallback(
    (_: React.MouseEvent, edge: { id: string }) => {
      setSelectedEdge(edge.id);
    },
    [setSelectedEdge]
  );

  const handlePaneClick = useCallback(() => {
    setSelectedNode(null);
    setSelectedEdge(null);
  }, [setSelectedNode, setSelectedEdge]);

  const defaultEdgeOptions = useMemo(
    () => ({
      type: "archEdge" as const,
      animated: true,
    }),
    []
  );

  return (
    <div className={styles.canvasContainer}>
      <ReactFlow
        nodes={nodes}
        edges={edges}
        onNodesChange={onNodesChange}
        onEdgesChange={onEdgesChange}
        onConnect={onConnect}
        onNodeClick={handleNodeClick}
        onEdgeClick={handleEdgeClick}
        onPaneClick={handlePaneClick}
        nodeTypes={nodeTypes}
        edgeTypes={edgeTypes}
        defaultEdgeOptions={defaultEdgeOptions}
        fitView
        snapToGrid
        snapGrid={[20, 20]}
        minZoom={0.1}
        maxZoom={2}
        proOptions={{ hideAttribution: true }}
        className={styles.reactFlow}
      >
        <Background
          variant={BackgroundVariant.Dots}
          gap={20}
          size={1}
          color="var(--color-border)"
        />
        {isMinimapVisible && (
          <MiniMap
            nodeColor={(node) => {
              const category = (node.data as { category?: string })?.category;
              const colors: Record<string, string> = {
                frontend: "#22d3ee",
                backend: "#a78bfa",
                database: "#fbbf24",
                infrastructure: "#34d399",
                ai: "#f472b6",
                integration: "#818cf8",
                custom: "#9ca3af",
              };
              return colors[category ?? "custom"] ?? "#9ca3af";
            }}
            maskColor="rgba(0, 0, 0, 0.7)"
            className={styles.minimap}
            pannable
            zoomable
          />
        )}
      </ReactFlow>

      {/* Overlays */}
      <NodePalette />
      <CanvasToolbar />

      {/* Empty state */}
      {nodes.length === 0 && (
        <div className={styles.emptyState}>
          <div className={styles.emptyIcon}>🎨</div>
          <h3 className={styles.emptyTitle}>Start Building</h3>
          <p className={styles.emptyText}>
            Click a component from the palette to add it to the canvas,
            or load the demo architecture with the ⚡ button.
          </p>
        </div>
      )}
    </div>
  );
}

/**
 * CanvasEditor wraps the React Flow canvas in the required provider.
 * Must be rendered client-side only (no SSR).
 */
export default function CanvasEditor() {
  return (
    <ReactFlowProvider>
      <CanvasEditorInner />
    </ReactFlowProvider>
  );
}
