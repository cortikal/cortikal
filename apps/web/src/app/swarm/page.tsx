"use client";

import React from "react";
import AppShell from "../../components/layout/AppShell";
import AgentChatPanel from "../../components/swarm/AgentChatPanel";
import FileTreePanel from "../../components/swarm/FileTreePanel";
import CodePreviewPanel from "../../components/swarm/CodePreviewPanel";

export default function SwarmPage() {
  return (
    <AppShell>
      <div style={{ display: "flex", height: "100%", width: "100%", overflow: "hidden" }}>
        
        {/* Left Panel: Swarm Chat */}
        <div style={{ width: "350px", flexShrink: 0 }}>
          <AgentChatPanel />
        </div>

        {/* Middle Panel: File Tree */}
        <div style={{ width: "250px", flexShrink: 0, background: "var(--color-surface)" }}>
          <FileTreePanel />
        </div>

        {/* Right Panel: Code Preview */}
        <div style={{ flex: 1, minWidth: 0 }}>
          <CodePreviewPanel />
        </div>
        
      </div>
    </AppShell>
  );
}
