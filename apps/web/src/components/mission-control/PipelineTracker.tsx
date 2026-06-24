"use client";

import React, { useEffect, useState } from "react";
import { ApiClient as api } from "../../lib/api";

interface PipelineStage {
  name: string;
  status: number; // 0=Queued, 1=Building, 2=Testing, 3=Deploying, 4=Success, 5=Failed, 6=Cancelled
  outputLog: string;
  duration: string;
}

interface Pipeline {
  id: string;
  name: string;
  status: number;
  startedAt: string;
  completedAt?: string;
  stages: PipelineStage[];
}

export default function PipelineTracker({ projectId }: { projectId: string }) {
  const [pipelines, setPipelines] = useState<Pipeline[]>([]);

  const fetchPipelines = () => {
    api.missionControl.getPipelines(projectId)
      .then(setPipelines)
      .catch(console.error);
  };

  useEffect(() => {
    fetchPipelines();
    // Poll for pipeline updates
    const interval = setInterval(fetchPipelines, 2000);
    return () => clearInterval(interval);
  }, [projectId]);

  const handleTriggerBuild = async () => {
    await api.missionControl.queuePipeline(projectId, "Manual Trigger Build");
    fetchPipelines();
  };

  const getStatusColor = (status: number) => {
    switch(status) {
      case 0: return "var(--color-text-secondary)"; // Queued
      case 1: case 2: case 3: return "var(--color-info)"; // In Progress
      case 4: return "var(--color-success)"; // Success
      case 5: return "var(--color-error)"; // Failed
      default: return "var(--color-text-muted)";
    }
  };

  const getStatusText = (status: number) => {
    const states = ["Queued", "Building", "Testing", "Deploying", "Success", "Failed", "Cancelled"];
    return states[status] || "Unknown";
  };

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: "var(--space-4)" }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <h3 style={{ margin: 0 }}>CI/CD Pipelines</h3>
        <button 
          onClick={handleTriggerBuild}
          style={{ 
            background: "var(--color-accent)", 
            color: "white", 
            border: "none", 
            padding: "8px 16px", 
            borderRadius: "var(--radius-md)", 
            cursor: "pointer",
            fontWeight: "bold"
          }}
        >
          🚀 Trigger Build
        </button>
      </div>

      <div style={{ display: "flex", flexDirection: "column", gap: "var(--space-4)" }}>
        {pipelines.length === 0 ? (
          <div style={{ padding: "var(--space-8)", textAlign: "center", color: "var(--color-text-muted)", background: "var(--color-surface)", borderRadius: "var(--radius-lg)", border: "1px dashed var(--color-border)" }}>
            No pipelines executed yet. Click "Trigger Build" to start a test run.
          </div>
        ) : (
          pipelines.map((p) => (
            <div key={p.id} style={{ background: "var(--color-surface)", padding: "var(--space-4)", borderRadius: "var(--radius-lg)", border: "1px solid var(--color-border)" }}>
              <div style={{ display: "flex", justifyContent: "space-between", marginBottom: "var(--space-4)" }}>
                <div>
                  <div style={{ fontWeight: 600, color: "var(--color-text-primary)" }}>{p.name}</div>
                  <div style={{ fontSize: "0.8rem", color: "var(--color-text-secondary)", fontFamily: "var(--font-mono)", marginTop: "4px" }}>
                    ID: {p.id.split('-')[0]} • Started: {new Date(p.startedAt).toLocaleTimeString()}
                  </div>
                </div>
                <div style={{ 
                  padding: "4px 12px", 
                  borderRadius: "16px", 
                  fontSize: "0.85rem", 
                  fontWeight: 600,
                  background: `${getStatusColor(p.status)}22`, // 20% opacity
                  color: getStatusColor(p.status) 
                }}>
                  {getStatusText(p.status)}
                </div>
              </div>

              {/* Stages row */}
              <div style={{ display: "flex", gap: "var(--space-2)" }}>
                {p.stages.map((s, i) => (
                  <div key={i} style={{ flex: 1, display: "flex", flexDirection: "column", gap: "4px" }}>
                    <div style={{ 
                      height: "4px", 
                      borderRadius: "2px", 
                      background: getStatusColor(s.status),
                      opacity: s.status === 0 ? 0.2 : 1
                    }} />
                    <div style={{ fontSize: "0.75rem", color: "var(--color-text-secondary)", textAlign: "center" }}>
                      {s.name}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
