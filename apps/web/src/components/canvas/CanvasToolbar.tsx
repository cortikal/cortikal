"use client";

import React from "react";
import { useReactFlow } from "@xyflow/react";
import { useCanvasStore } from "../../stores/canvasStore";
import { archToReactFlow, demoGraph } from "../../utils/archConverter";
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

  const handleExport = () => {
    const data = JSON.stringify({ nodes, edges }, null, 2);
    const blob = new Blob([data], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "architecture.json";
    a.click();
    URL.revokeObjectURL(url);
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
      </div>
      <div className={styles.group}>
        <button
          className={styles.btn}
          onClick={handleExport}
          title="Export architecture"
          id="toolbar-export"
        >
          📥
        </button>
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
