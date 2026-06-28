"use client";

import React, { useRef } from "react";
import { useReactFlow } from "@xyflow/react";
import { useCanvasStore } from "../../stores/canvasStore";
import {
  archToReactFlow,
  demoGraph,
  reactFlowToArch,
} from "../../utils/archConverter";
import { ApiClient } from "../../lib/api";
import styles from "./CanvasToolbar.module.css";

export default function CanvasToolbar() {
  const { zoomIn, zoomOut, fitView } = useReactFlow();
  const togglePalette = useCanvasStore((s) => s.togglePalette);
  const toggleMinimap = useCanvasStore((s) => s.toggleMinimap);
  const isPaletteOpen = useCanvasStore((s) => s.isPaletteOpen);
  const isMinimapVisible = useCanvasStore((s) => s.isMinimapVisible);
  const clearGraph = useCanvasStore((s) => s.clearGraph);
  const loadGraph = useCanvasStore((s) => s.loadGraph);
  const nodes = useCanvasStore((s) => s.nodes);
  const edges = useCanvasStore((s) => s.edges);

  const fileInputRef = useRef<HTMLInputElement>(null);

  // Export current canvas as JSON (quick save)
  const handleExportJson = () => {
    const data = JSON.stringify({ nodes, edges }, null, 2);
    const blob = new Blob([data], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "architecture.json";
    a.click();
    URL.revokeObjectURL(url);
  };

  // Save canvas as arch.md via the backend serializer
  const handleSaveArchMd = async () => {
    if (nodes.length === 0) return;
    try {
      const graph = reactFlowToArch(nodes, edges);
      const archDocument = {
        metadata: {
          name: "My Architecture",
          author: "cortikal",
          version: "0.1.0",
          tags: [],
          complexity: "moderate",
          description: "",
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        },
        graph,
        description: null,
      };
      const markdown = await ApiClient.serializeArchMd(archDocument);
      const blob = new Blob([markdown], { type: "text/markdown" });
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = "arch.md";
      a.click();
      URL.revokeObjectURL(url);
    } catch (err) {
      console.error("Failed to save arch.md:", err);
      alert("Failed to save arch.md. Is the backend running?");
    }
  };

  // Import arch.md from a local file
  const handleImportArchMd = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = async (ev) => {
      const content = ev.target?.result as string;
      try {
        const result = await ApiClient.parseArchMd(content);
        const graphData = result.document?.graph || result.graph || result;
        const { nodes: n, edges: ed } = archToReactFlow(graphData);
        loadGraph(n, ed);
      } catch (err) {
        console.error("Failed to import arch.md:", err);
        alert("Failed to parse the selected arch.md file.");
      }
    };
    reader.readAsText(file);
    // Reset file input so same file can be re-selected
    e.target.value = "";
  };

  return (
    <div className={styles.toolbar} id="canvas-toolbar">
      {/* Left group */}
      <div className={styles.group}>
        <button
          className={`${styles.btn} ${isPaletteOpen ? styles.active : ""}`}
          onClick={togglePalette}
          title="Toggle component palette"
          id="toolbar-toggle-palette"
        >
          🧩
        </button>
        <div className={styles.divider} />
        <button
          className={styles.btn}
          onClick={() => zoomIn({ duration: 200 })}
          title="Zoom in"
          id="toolbar-zoom-in"
        >
          +
        </button>
        <button
          className={styles.btn}
          onClick={() => zoomOut({ duration: 200 })}
          title="Zoom out"
          id="toolbar-zoom-out"
        >
          −
        </button>
        <button
          className={styles.btn}
          onClick={() => fitView({ duration: 300, padding: 0.2 })}
          title="Fit to view"
          id="toolbar-fit"
        >
          ⊞
        </button>
        <div className={styles.divider} />
        <button
          className={`${styles.btn} ${isMinimapVisible ? styles.active : ""}`}
          onClick={toggleMinimap}
          title="Toggle minimap"
          id="toolbar-minimap"
        >
          🗺️
        </button>
      </div>

      {/* Center — stats */}
      <div className={styles.stats}>
        <span className={styles.stat}>
          <span className={styles.statValue}>{nodes.length}</span> nodes
        </span>
        <span className={styles.statDot}>·</span>
        <span className={styles.stat}>
          <span className={styles.statValue}>{edges.length}</span> edges
        </span>
      </div>

      {/* Right group */}
      <div className={styles.group}>
        <button
          className={styles.btn}
          onClick={() => {
            const { nodes: n, edges: e } = archToReactFlow(demoGraph);
            loadGraph(n, e);
          }}
          title="Load demo architecture"
          id="toolbar-load-demo"
        >
          ⚡
        </button>
        <div className={styles.divider} />
        {/* Import arch.md */}
        <input
          ref={fileInputRef}
          type="file"
          accept=".md,.arch.md"
          style={{ display: "none" }}
          onChange={handleImportArchMd}
          id="toolbar-import-input"
        />
        <button
          className={styles.btn}
          onClick={() => fileInputRef.current?.click()}
          title="Import arch.md file"
          id="toolbar-import"
        >
          📂
        </button>
        {/* Save as arch.md */}
        <button
          className={styles.btn}
          onClick={handleSaveArchMd}
          title="Save as arch.md"
          id="toolbar-save-arch"
          disabled={nodes.length === 0}
        >
          💾
        </button>
        {/* Export JSON */}
        <button
          className={styles.btn}
          onClick={handleExportJson}
          title="Export as JSON"
          id="toolbar-export"
        >
          📥
        </button>
        <div className={styles.divider} />
        <button
          className={`${styles.btn} ${styles.danger}`}
          onClick={clearGraph}
          title="Clear canvas"
          id="toolbar-clear"
        >
          🗑️
        </button>
      </div>
    </div>
  );
}
