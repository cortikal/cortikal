"use client";

import React, { useEffect, useState } from "react";
import AppShell from "../../components/layout/AppShell";
import AgentChatPanel from "../../components/swarm/AgentChatPanel";
import FileTreePanel from "../../components/swarm/FileTreePanel";
import CodePreviewPanel from "../../components/swarm/CodePreviewPanel";
import { agentHub } from "../../lib/api";

export interface SwarmFile {
  name: string;
  content: string;
}

export default function SwarmPage() {
  const [files, setFiles] = useState<SwarmFile[]>([]);
  const [selectedFile, setSelectedFile] = useState<SwarmFile | null>(null);

  useEffect(() => {
    const onAgentMessage = (msg: any) => {
      if (msg.generatedFiles && msg.generatedFiles.length > 0) {
        setFiles(prev => {
          const newFiles = [...prev];
          msg.generatedFiles.forEach((gf: any) => {
            const idx = newFiles.findIndex(f => f.name === gf.filePath);
            if (idx >= 0) newFiles[idx] = { name: gf.filePath, content: gf.content };
            else newFiles.push({ name: gf.filePath, content: gf.content });
          });
          return newFiles;
        });
      }
    };
    
    agentHub.connect().then(() => {
      agentHub.onAgentMessage(onAgentMessage);
    });

    return () => agentHub.offAgentMessage(onAgentMessage);
  }, []);

  return (
    <AppShell>
      <div style={{ display: "flex", height: "100%", width: "100%", overflow: "hidden" }}>
        
        {/* Left Panel: Swarm Chat */}
        <div style={{ width: "350px", flexShrink: 0 }}>
          <AgentChatPanel />
        </div>

        {/* Middle Panel: File Tree */}
        <div style={{ width: "250px", flexShrink: 0, background: "var(--color-surface)" }}>
          <FileTreePanel files={files} onSelectFile={setSelectedFile} selectedFile={selectedFile} />
        </div>

        {/* Right Panel: Code Preview */}
        <div style={{ flex: 1, minWidth: 0 }}>
          <CodePreviewPanel selectedFile={selectedFile} />
        </div>
        
      </div>
    </AppShell>
  );
}
