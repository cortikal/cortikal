"use client";

import React, { memo } from "react";
import { Handle, Position, type NodeProps } from "@xyflow/react";
import type { ArchNode, NodeCategory } from "../../stores/canvasStore";
import styles from "./ArchNode.module.css";

const categoryIcons: Record<NodeCategory, string> = {
  frontend: "🌐",
  backend: "⚙️",
  database: "🗄️",
  infrastructure: "☁️",
  ai: "🧠",
  integration: "🔌",
  custom: "📦",
};

function ArchNodeComponent({ data, selected }: NodeProps<ArchNode>) {
  return (
    <div
      className={`${styles.archNode} ${styles[data.category]}`}
      data-selected={selected}
      data-category={data.category}
    >
      {/* Input handles */}
      {data.inputs.map((port, i) => (
        <Handle
          key={port.id}
          type="target"
          position={Position.Left}
          id={port.id}
          className={styles.handle}
          style={{
            top: `${((i + 1) / (data.inputs.length + 1)) * 100}%`,
          }}
          title={`${port.label} (${port.dataType})`}
        />
      ))}

      {/* Node header */}
      <div className={styles.header}>
        <span className={styles.icon}>
          {categoryIcons[data.category] ?? "📦"}
        </span>
        <span className={styles.type}>{data.type}</span>
      </div>

      {/* Node body */}
      <div className={styles.body}>
        <div className={styles.label}>{data.label}</div>
      </div>

      {/* Port indicators */}
      <div className={styles.ports}>
        {data.inputs.length > 0 && (
          <div className={styles.portGroup}>
            {data.inputs.map((port) => (
              <div key={port.id} className={styles.portLabel} title={port.description}>
                <span className={styles.portDot} data-direction="input" />
                <span className={styles.portText}>{port.label}</span>
                <span className={styles.portType}>{port.dataType}</span>
              </div>
            ))}
          </div>
        )}
        {data.outputs.length > 0 && (
          <div className={`${styles.portGroup} ${styles.outputGroup}`}>
            {data.outputs.map((port) => (
              <div key={port.id} className={styles.portLabel} title={port.description}>
                <span className={styles.portType}>{port.dataType}</span>
                <span className={styles.portText}>{port.label}</span>
                <span className={styles.portDot} data-direction="output" />
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Output handles */}
      {data.outputs.map((port, i) => (
        <Handle
          key={port.id}
          type="source"
          position={Position.Right}
          id={port.id}
          className={styles.handle}
          style={{
            top: `${((i + 1) / (data.outputs.length + 1)) * 100}%`,
          }}
          title={`${port.label} (${port.dataType})`}
        />
      ))}
    </div>
  );
}

export default memo(ArchNodeComponent);
