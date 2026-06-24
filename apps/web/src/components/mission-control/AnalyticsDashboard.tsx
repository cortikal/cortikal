"use client";

import React, { useEffect, useState } from "react";
import { ApiClient as api } from "../../lib/api";

interface ProjectStats {
  totalTokensUsed: number;
  successfulBuilds: number;
  failedBuilds: number;
  healthScore: number;
}

export default function AnalyticsDashboard({ projectId }: { projectId: string }) {
  const [stats, setStats] = useState<ProjectStats | null>(null);

  useEffect(() => {
    // Initial fetch
    api.missionControl.getStats(projectId)
      .then(setStats)
      .catch(console.error);

    // Mock polling for live stats updates
    const interval = setInterval(() => {
      api.missionControl.getStats(projectId).then(setStats);
    }, 5000);

    return () => clearInterval(interval);
  }, [projectId]);

  if (!stats) return <div style={{ color: "var(--color-text-muted)" }}>Loading metrics...</div>;

  return (
    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr 1fr", gap: "var(--space-4)", marginBottom: "var(--space-8)" }}>
      {/* Token Usage */}
      <div style={{ background: "var(--color-surface)", padding: "var(--space-4)", borderRadius: "var(--radius-lg)", border: "1px solid var(--color-border)" }}>
        <div style={{ fontSize: "0.85rem", color: "var(--color-text-secondary)", marginBottom: "var(--space-2)" }}>Total Tokens</div>
        <div style={{ fontSize: "1.8rem", fontWeight: "600", color: "var(--color-text-primary)" }}>
          {stats.totalTokensUsed.toLocaleString()}
        </div>
      </div>
      
      {/* Health Score */}
      <div style={{ background: "var(--color-surface)", padding: "var(--space-4)", borderRadius: "var(--radius-lg)", border: "1px solid var(--color-border)" }}>
        <div style={{ fontSize: "0.85rem", color: "var(--color-text-secondary)", marginBottom: "var(--space-2)" }}>Project Health</div>
        <div style={{ fontSize: "1.8rem", fontWeight: "600", color: stats.healthScore > 80 ? "var(--color-success)" : "var(--color-warning)" }}>
          {stats.healthScore.toFixed(1)}%
        </div>
      </div>

      {/* Success */}
      <div style={{ background: "var(--color-surface)", padding: "var(--space-4)", borderRadius: "var(--radius-lg)", border: "1px solid var(--color-border)" }}>
        <div style={{ fontSize: "0.85rem", color: "var(--color-text-secondary)", marginBottom: "var(--space-2)" }}>Successful Builds</div>
        <div style={{ fontSize: "1.8rem", fontWeight: "600", color: "var(--color-success)" }}>
          {stats.successfulBuilds}
        </div>
      </div>

      {/* Failed */}
      <div style={{ background: "var(--color-surface)", padding: "var(--space-4)", borderRadius: "var(--radius-lg)", border: "1px solid var(--color-border)" }}>
        <div style={{ fontSize: "0.85rem", color: "var(--color-text-secondary)", marginBottom: "var(--space-2)" }}>Failed Builds</div>
        <div style={{ fontSize: "1.8rem", fontWeight: "600", color: "var(--color-error)" }}>
          {stats.failedBuilds}
        </div>
      </div>
    </div>
  );
}
