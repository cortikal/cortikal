"use client";

import { SwarmFile } from "../../app/swarm/page";

interface FileTreePanelProps {
  files: SwarmFile[];
  onSelectFile: (file: SwarmFile) => void;
  selectedFile: SwarmFile | null;
}

export default function FileTreePanel({ files, onSelectFile, selectedFile }: FileTreePanelProps) {
  // Sort files alphabetically
  const sortedFiles = [...files].sort((a, b) => a.name.localeCompare(b.name));

  return (
    <div style={{ height: "100%", borderRight: "1px solid var(--color-border)", display: "flex", flexDirection: "column" }}>
      <div style={{ padding: "var(--space-4)", borderBottom: "1px solid var(--color-border)" }}>
        <h3 style={{ margin: 0, fontSize: "0.9rem", color: "var(--color-text-secondary)" }}>EXPLORER</h3>
      </div>
      <div style={{ flex: 1, overflowY: "auto", padding: "var(--space-2)" }}>
        {sortedFiles.length === 0 ? (
          <div style={{ color: "var(--color-text-muted)", fontSize: "0.85rem", padding: "var(--space-2)" }}>
            No files generated yet.
          </div>
        ) : (
          sortedFiles.map((f, i) => (
            <div 
              key={i} 
              onClick={() => onSelectFile(f)}
              style={{ 
                padding: "var(--space-2) var(--space-4)", 
                cursor: "pointer", 
                fontFamily: "var(--font-mono)", 
                fontSize: "0.85rem",
                color: selectedFile?.name === f.name ? "var(--color-accent)" : "var(--color-text-primary)",
                background: selectedFile?.name === f.name ? "var(--color-surface-hover)" : "transparent",
                whiteSpace: "pre"
              }}
            >
              📄 {f.name}
            </div>
          ))
        )}
      </div>
    </div>
  );
}
