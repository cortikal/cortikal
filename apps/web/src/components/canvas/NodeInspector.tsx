"use client";

import React, { useState } from "react";
import { useCanvasStore } from "../../stores/canvasStore";
import styles from "./NodeInspector.module.css";

const categoryIcons: Record<string, string> = {
  frontend: "🌐",
  backend: "⚙️",
  database: "🗄️",
  infrastructure: "☁️",
  ai: "🧠",
  integration: "🔌",
  custom: "📦",
};

const dataTypeColors: Record<string, string> = {
  http: "var(--color-accent)",
  json: "var(--color-success)",
  sql: "var(--color-warning)",
  grpc: "var(--color-secondary)",
  event: "#f472b6",
  stream: "#22d3ee",
  ws: "#a78bfa",
};

export default function NodeInspector() {
  const selectedNodeId = useCanvasStore((s) => s.selectedNodeId);
  const nodes = useCanvasStore((s) => s.nodes);
  const updateNodeData = useCanvasStore((s) => s.updateNodeData);
  const removeNode = useCanvasStore((s) => s.removeNode);
  const setSelectedNode = useCanvasStore((s) => s.setSelectedNode);

  const node = nodes.find((n) => n.id === selectedNodeId) ?? null;

  // Initialise from node.data.label; the parent passes key={selectedNodeId}
  // so this component remounts whenever the selection changes.
  const [labelDraft, setLabelDraft] = useState(node?.data.label ?? "");

  if (!node) return null;

  const handleLabelSave = () => {
    if (labelDraft.trim() && labelDraft.trim() !== node.data.label) {
      updateNodeData(node.id, { label: labelDraft.trim() });
    }
  };

  const handleDelete = () => {
    removeNode(node.id);
    setSelectedNode(null);
  };

  return (
    <div className={styles.inspector}>
      {/* Header */}
      <div className={styles.header}>
        <div className={styles.headerLeft}>
          <span className={styles.icon}>
            {categoryIcons[node.data.category] ?? "📦"}
          </span>
          <div>
            <div className={styles.category}>{node.data.category}</div>
            <div className={styles.nodeType}>{node.data.type}</div>
          </div>
        </div>
        <button
          className={styles.closeBtn}
          onClick={() => setSelectedNode(null)}
          title="Close inspector"
        >
          ✕
        </button>
      </div>

      {/* Label editor */}
      <div className={styles.section}>
        <label className={styles.sectionLabel}>Label</label>
        <input
          className={styles.labelInput}
          value={labelDraft}
          onChange={(e) => setLabelDraft(e.target.value)}
          onBlur={handleLabelSave}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              handleLabelSave();
              (e.target as HTMLInputElement).blur();
            }
          }}
        />
      </div>

      {/* ID (read-only) */}
      <div className={styles.section}>
        <label className={styles.sectionLabel}>ID</label>
        <div className={styles.idBadge}>{node.id}</div>
      </div>

      {/* Position */}
      <div className={styles.section}>
        <label className={styles.sectionLabel}>Position</label>
        <div className={styles.positionRow}>
          <span className={styles.posItem}>
            x: <strong>{Math.round(node.position.x)}</strong>
          </span>
          <span className={styles.posItem}>
            y: <strong>{Math.round(node.position.y)}</strong>
          </span>
        </div>
      </div>

      {/* Input ports */}
      {node.data.inputs.length > 0 && (
        <div className={styles.section}>
          <label className={styles.sectionLabel}>
            Inputs ({node.data.inputs.length})
          </label>
          <div className={styles.portList}>
            {node.data.inputs.map((port) => (
              <div key={port.id} className={styles.portItem}>
                <span
                  className={styles.portDot}
                  style={{
                    background:
                      dataTypeColors[port.dataType] ??
                      "var(--color-text-muted)",
                  }}
                />
                <span className={styles.portName}>{port.label}</span>
                <span className={styles.portType}>{port.dataType}</span>
                {port.required && <span className={styles.required}>req</span>}
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Output ports */}
      {node.data.outputs.length > 0 && (
        <div className={styles.section}>
          <label className={styles.sectionLabel}>
            Outputs ({node.data.outputs.length})
          </label>
          <div className={styles.portList}>
            {node.data.outputs.map((port) => (
              <div key={port.id} className={styles.portItem}>
                <span
                  className={styles.portDot}
                  style={{
                    background:
                      dataTypeColors[port.dataType] ??
                      "var(--color-text-muted)",
                  }}
                />
                <span className={styles.portName}>{port.label}</span>
                <span className={styles.portType}>{port.dataType}</span>
                {port.required && <span className={styles.required}>req</span>}
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Config */}
      {Object.keys(node.data.config).length > 0 && (
        <div className={styles.section}>
          <label className={styles.sectionLabel}>Config</label>
          <pre className={styles.configPre}>
            {JSON.stringify(node.data.config, null, 2)}
          </pre>
        </div>
      )}

      {/* Footer */}
      <div className={styles.footer}>
        <button className={styles.deleteBtn} onClick={handleDelete}>
          🗑 Delete Node
        </button>
      </div>
    </div>
  );
}
