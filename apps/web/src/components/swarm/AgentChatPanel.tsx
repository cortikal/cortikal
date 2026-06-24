"use client";

import React, { useEffect, useState } from "react";
import { agentHub, orchestratorHub } from "../../lib/api";

interface ChatMessage {
  id: string;
  role: string;
  content: string;
  timestamp: string;
}

const roleColors: Record<string, string> = {
  Architect: "var(--color-accent)",
  FrontendDev: "var(--color-info)",
  BackendDev: "var(--color-success)",
  QA: "var(--color-warning)",
  System: "var(--color-text-secondary)",
};

export default function AgentChatPanel() {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [orchestratorState, setOrchestratorState] = useState<string>("Idle");

  useEffect(() => {
    // Connect to hubs
    orchestratorHub.connect().catch(console.error);
    agentHub.connect().catch(console.error);

    const onStateUpdate = (state: string, message: string) => {
      setOrchestratorState(state);
      setMessages((prev) => [
        ...prev,
        {
          id: Date.now().toString() + Math.random().toString(),
          role: "System",
          content: `[${state}] ${message}`,
          timestamp: new Date().toISOString(),
        },
      ]);
    };

    const onAgentMessage = (msg: any) => {
      setMessages((prev) => [
        ...prev,
        {
          id: msg.id || Date.now().toString(),
          role: msg.agentRole,
          content: msg.content,
          timestamp: msg.timestamp,
        },
      ]);
    };

    orchestratorHub.onStateUpdate(onStateUpdate);
    agentHub.onAgentMessage(onAgentMessage);

    return () => {
      orchestratorHub.offStateUpdate(onStateUpdate);
      agentHub.offAgentMessage(onAgentMessage);
    };
  }, []);

  return (
    <div style={{ display: "flex", flexDirection: "column", height: "100%", borderRight: "1px solid var(--color-border)" }}>
      {/* Header */}
      <div style={{ padding: "var(--space-4)", borderBottom: "1px solid var(--color-border)", display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <h3 style={{ margin: 0 }}>Swarm Chat</h3>
        <span style={{ 
          fontSize: "0.8rem", 
          padding: "2px 8px", 
          borderRadius: "12px", 
          background: orchestratorState === "Idle" ? "var(--color-surface-hover)" : "var(--color-accent)",
          color: orchestratorState === "Idle" ? "var(--color-text-secondary)" : "#fff"
        }}>
          {orchestratorState}
        </span>
      </div>

      {/* Messages */}
      <div style={{ flex: 1, overflowY: "auto", padding: "var(--space-4)", display: "flex", flexDirection: "column", gap: "var(--space-4)" }}>
        {messages.length === 0 ? (
          <div style={{ color: "var(--color-text-muted)", textAlign: "center", marginTop: "var(--space-8)" }}>
            No activity yet. Awaiting orchestrator start.
          </div>
        ) : (
          messages.map((m) => (
            <div key={m.id} style={{ display: "flex", flexDirection: "column", gap: "4px" }}>
              <span style={{ fontSize: "0.8rem", color: roleColors[m.role] || "var(--color-text-secondary)", fontWeight: 600 }}>
                {m.role}
              </span>
              <div style={{ 
                background: m.role === "System" ? "transparent" : "var(--color-surface-hover)", 
                padding: m.role === "System" ? "0" : "var(--space-3)", 
                borderRadius: "var(--radius-md)",
                fontSize: m.role === "System" ? "0.9rem" : "1rem",
                color: m.role === "System" ? "var(--color-text-secondary)" : "var(--color-text-primary)",
                fontStyle: m.role === "System" ? "italic" : "normal"
              }}>
                {m.content}
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
