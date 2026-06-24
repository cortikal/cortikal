import React from "react";
import AppShell from "../../components/layout/AppShell";
import AnalyticsDashboard from "../../components/mission-control/AnalyticsDashboard";
import PipelineTracker from "../../components/mission-control/PipelineTracker";

export default function MissionControlPage() {
  // Hardcoded project ID for the Phase 5 scaffold
  const currentProjectId = "project-alpha-123";

  return (
    <AppShell>
      <div style={{ maxWidth: "1200px", margin: "0 auto", padding: "var(--space-8)" }}>
        
        <div style={{ marginBottom: "var(--space-8)" }}>
          <h1 style={{ margin: 0, fontSize: "2.5rem", color: "var(--color-text-primary)" }}>Mission Control</h1>
          <p style={{ color: "var(--color-text-secondary)", fontSize: "1.1rem" }}>
            Telemetry, CI/CD Pipelines, and Agent Swarm analytics.
          </p>
        </div>

        {/* Top metrics dashboard */}
        <AnalyticsDashboard projectId={currentProjectId} />

        {/* Main layout: Pipelines on left, Deployments on right */}
        <div style={{ display: "grid", gridTemplateColumns: "2fr 1fr", gap: "var(--space-8)" }}>
          
          {/* Pipelines */}
          <PipelineTracker projectId={currentProjectId} />

          {/* Deployments (Mock) */}
          <div style={{ display: "flex", flexDirection: "column", gap: "var(--space-4)" }}>
            <h3 style={{ margin: 0 }}>Active Deployments</h3>
            
            <div style={{ background: "var(--color-surface)", padding: "var(--space-4)", borderRadius: "var(--radius-lg)", border: "1px solid var(--color-border)" }}>
              <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: "var(--space-2)" }}>
                <div style={{ fontWeight: 600 }}>Production</div>
                <div style={{ display: "flex", alignItems: "center", gap: "6px", fontSize: "0.8rem", color: "var(--color-success)" }}>
                  <div style={{ width: "8px", height: "8px", borderRadius: "50%", background: "var(--color-success)" }} />
                  Healthy
                </div>
              </div>
              <a href="#" style={{ color: "var(--color-accent)", fontSize: "0.9rem", textDecoration: "none" }}>https://alpha.cortikal.dev</a>
              <div style={{ fontSize: "0.8rem", color: "var(--color-text-secondary)", marginTop: "var(--space-2)" }}>Last deployed: 2 hours ago</div>
            </div>

            <div style={{ background: "var(--color-surface)", padding: "var(--space-4)", borderRadius: "var(--radius-lg)", border: "1px solid var(--color-border)" }}>
              <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: "var(--space-2)" }}>
                <div style={{ fontWeight: 600 }}>Staging</div>
                <div style={{ display: "flex", alignItems: "center", gap: "6px", fontSize: "0.8rem", color: "var(--color-warning)" }}>
                  <div style={{ width: "8px", height: "8px", borderRadius: "50%", background: "var(--color-warning)" }} />
                  Degraded
                </div>
              </div>
              <a href="#" style={{ color: "var(--color-accent)", fontSize: "0.9rem", textDecoration: "none" }}>https://staging-alpha.cortikal.dev</a>
              <div style={{ fontSize: "0.8rem", color: "var(--color-text-secondary)", marginTop: "var(--space-2)" }}>Last deployed: 14 mins ago</div>
            </div>
            
          </div>

        </div>

      </div>
    </AppShell>
  );
}
