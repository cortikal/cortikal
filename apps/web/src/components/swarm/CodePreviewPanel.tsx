"use client";

import { SwarmFile } from "../../app/swarm/page";

interface CodePreviewPanelProps {
  selectedFile: SwarmFile | null;
}

export default function CodePreviewPanel({ selectedFile }: CodePreviewPanelProps) {
  if (!selectedFile) {
    return (
      <div style={{ display: "flex", alignItems: "center", justifyContent: "center", height: "100%", background: "#1e1e1e", color: "var(--color-text-muted)" }}>
        Select a file to preview
      </div>
    );
  }

  return (
    <div style={{ display: "flex", flexDirection: "column", height: "100%", background: "#1e1e1e" }}>
      <div style={{ padding: "var(--space-4)", borderBottom: "1px solid var(--color-border)" }}>
        <div style={{ display: "flex", gap: "var(--space-2)" }}>
          <span style={{ fontFamily: "var(--font-mono)", fontSize: "0.9rem", color: "var(--color-text-secondary)" }}>
            {selectedFile.name}
          </span>
        </div>
      </div>
      <div style={{ flex: 1, overflowY: "auto", padding: "var(--space-4)" }}>
        <pre style={{ margin: 0, fontFamily: "var(--font-mono)", fontSize: "0.9rem", color: "#d4d4d4" }}>
          <code>{selectedFile.content}</code>
        </pre>
      </div>
    </div>
  );
}
