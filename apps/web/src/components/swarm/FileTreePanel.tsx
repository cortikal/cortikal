"use client";

import React from "react";

export default function FileTreePanel() {
  // Mock file tree for Phase 4 scaffold
  const mockFiles = [
    { name: "src/", isDir: true },
    { name: "  components/", isDir: true },
    { name: "    Button.tsx", isDir: false },
    { name: "    Header.tsx", isDir: false },
    { name: "  pages/", isDir: true },
    { name: "    index.tsx", isDir: false },
    { name: "package.json", isDir: false },
  ];

  return (
    <div style={{ height: "100%", borderRight: "1px solid var(--color-border)", display: "flex", flexDirection: "column" }}>
      <div style={{ padding: "var(--space-4)", borderBottom: "1px solid var(--color-border)" }}>
        <h3 style={{ margin: 0, fontSize: "0.9rem", color: "var(--color-text-secondary)" }}>EXPLORER</h3>
      </div>
      <div style={{ flex: 1, overflowY: "auto", padding: "var(--space-2)" }}>
        {mockFiles.map((f, i) => (
          <div key={i} style={{ 
            padding: "var(--space-2) var(--space-4)", 
            cursor: "pointer", 
            fontFamily: "var(--font-mono)", 
            fontSize: "0.85rem",
            color: f.isDir ? "var(--color-text-secondary)" : "var(--color-text-primary)",
            whiteSpace: "pre"
          }}>
            {f.isDir ? "📁 " : "📄 "}{f.name}
          </div>
        ))}
      </div>
    </div>
  );
}
