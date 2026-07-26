"use client";

import React, { useEffect, useState, Suspense } from "react";
import { useSearchParams } from "next/navigation";
import AppShell from "../../components/layout/AppShell";
import AgentChatPanel from "../../components/swarm/AgentChatPanel";
import FileTreePanel from "../../components/swarm/FileTreePanel";
import CodePreviewPanel from "../../components/swarm/CodePreviewPanel";
import { agentHub, ApiClient } from "../../lib/api";

export interface SwarmFile {
  name: string;
  content: string;
}

export default function SwarmPage() {
  return (
    <Suspense fallback={<AppShell><div>Loading...</div></AppShell>}>
      <SwarmContent />
    </Suspense>
  );
}

function SwarmContent() {
  const searchParams = useSearchParams();
  const projectId = searchParams.get("projectId") || "default-project";

  const [files, setFiles] = useState<SwarmFile[]>([]);
  const [selectedFile, setSelectedFile] = useState<SwarmFile | null>(null);
  const [initialMessages, setInitialMessages] = useState<any[]>([]);

  useEffect(() => {
    // Load existing transcript
    ApiClient.swarm.getTranscript(projectId).then(msgs => {
      setInitialMessages(msgs);
      const allFiles = new Map<string, string>();
      msgs.forEach((m: any) => {
        if (m.generatedFiles) {
          m.generatedFiles.forEach((gf: any) => {
            allFiles.set(gf.filePath, gf.content);
          });
        }
      });
      setFiles(Array.from(allFiles.entries()).map(([name, content]) => ({ name, content })));
    }).catch(console.error);
  }, [projectId]);

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
          <AgentChatPanel initialMessages={initialMessages} />
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
