"use client";

import React from "react";

export default function CodePreviewPanel() {
  const mockCode = `// Live code generation preview
import React from "react";

export default function GeneratedComponent() {
  return (
    <div className="p-4 border rounded shadow">
      <h2>Hello from Cortikal Swarm</h2>
      <button onClick={() => alert("Working!")}>
        Click Me
      </button>
    </div>
  );
}`;

  return (
    <div style={{ display: "flex", flexDirection: "column", height: "100%", background: "#1e1e1e" }}>
      <div style={{ padding: "var(--space-4)", borderBottom: "1px solid var(--color-border)" }}>
        <div style={{ display: "flex", gap: "var(--space-2)" }}>
          <span style={{ fontFamily: "var(--font-mono)", fontSize: "0.9rem", color: "var(--color-text-secondary)" }}>
            components/GeneratedComponent.tsx
          </span>
        </div>
      </div>
      <div style={{ flex: 1, overflowY: "auto", padding: "var(--space-4)" }}>
        <pre style={{ margin: 0, fontFamily: "var(--font-mono)", fontSize: "0.9rem", color: "#d4d4d4" }}>
          <code>{mockCode}</code>
        </pre>
      </div>
    </div>
  );
}
