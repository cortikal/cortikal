"use client";

import React, { useCallback, useMemo, useEffect, useState } from "react";
import { useSearchParams } from "next/navigation";
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
import { ApiClient } from "../../lib/api";
import { archToReactFlow } from "../../utils/archConverter";
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
  const loadGraph = useCanvasStore((s) => s.loadGraph);

  const searchParams = useSearchParams();
  const templateId = searchParams.get("template");
  const promptQuery = searchParams.get("prompt");
  const [isLoadingTemplate, setIsLoadingTemplate] = useState(!!templateId);

  useEffect(() => {
    if (templateId) {
      setIsLoadingTemplate(true);
      ApiClient.getTemplateContent(templateId)
        .then((content) => ApiClient.parseArchMd(content))
        .then((graph) => {
          const { nodes: n, edges: e } = archToReactFlow(graph);
          loadGraph(n, e);
          setIsLoadingTemplate(false);
        })
        .catch((err) => {
          console.error("Failed to load template:", err);
          setIsLoadingTemplate(false);
        });
    } else if (promptQuery) {
      // Future Phase: send prompt to LLM to generate architecture
      console.log("Generating architecture for prompt:", promptQuery);
    }
  }, [templateId, promptQuery, loadGraph]);

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
      {nodes.length === 0 && !isLoadingTemplate && (
        <div className={styles.emptyState}>
          <div className={styles.emptyIcon}>🎨</div>
          <h3 className={styles.emptyTitle}>Start Building</h3>
          <p className={styles.emptyText}>
            Click a component from the palette to add it to the canvas,
            or load the demo architecture with the ⚡ button.
          </p>
        </div>
      )}
      
      {isLoadingTemplate && (
        <div className={styles.emptyState}>
          <div className={styles.emptyIcon}>⏳</div>
          <h3 className={styles.emptyTitle}>Loading Template</h3>
          <p className={styles.emptyText}>Parsing architecture...</p>
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
