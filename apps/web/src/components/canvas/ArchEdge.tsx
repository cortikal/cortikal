"use client";

import React from "react";
import {
  BaseEdge,
  EdgeLabelRenderer,
  getBezierPath,
  type EdgeProps,
} from "@xyflow/react";
import type { ArchEdge } from "../../stores/canvasStore";
import styles from "./ArchEdge.module.css";

export default function ArchEdgeComponent({
  id,
  sourceX,
  sourceY,
  targetX,
  targetY,
  sourcePosition,
  targetPosition,
  data,
  selected,
}: EdgeProps<ArchEdge>) {
  const [edgePath, labelX, labelY] = getBezierPath({
    sourceX,
    sourceY,
    sourcePosition,
    targetX,
    targetY,
    targetPosition,
  });

  const isDependency = data?.edgeType === "dependency";

  return (
    <>
      <BaseEdge
        id={id}
        path={edgePath}
        className={`${styles.edge} ${isDependency ? styles.dependency : styles.dataflow}`}
        style={{
          stroke: selected ? "var(--color-accent)" : undefined,
        }}
      />
      {data?.label && (
        <EdgeLabelRenderer>
          <div
            className={styles.edgeLabel}
            style={{
              transform: `translate(-50%, -50%) translate(${labelX}px,${labelY}px)`,
            }}
            data-selected={selected}
          >
            <span className={styles.edgeLabelType}>{data.dataType}</span>
            <span className={styles.edgeLabelText}>{data.label}</span>
          </div>
        </EdgeLabelRenderer>
      )}
    </>
  );
}
