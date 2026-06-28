import {
  HubConnectionBuilder,
  HubConnection,
  LogLevel,
} from "@microsoft/signalr";

const API_BASE = "http://localhost:5050/api";
const HUB_BASE = "http://localhost:5050/hubs";

// --- REST API Client ---

export const ApiClient = {
  // Registry
  getTemplates: async () => {
    const res = await fetch(`${API_BASE}/registry/templates`);
    if (!res.ok) throw new Error("Failed to fetch templates");
    return res.json();
  },
  getTemplateContent: async (id: string) => {
    const res = await fetch(`${API_BASE}/registry/templates/${id}/content`);
    if (!res.ok) throw new Error("Failed to fetch template content");
    return res.text();
  },
  // Canvas
  parseArchMd: async (markdown: string) => {
    const res = await fetch(`${API_BASE}/canvas/parse`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(markdown),
    });
    if (!res.ok) throw new Error("Failed to parse arch.md");
    return res.json();
  },
  serializeArchMd: async (document: unknown) => {
    const res = await fetch(`${API_BASE}/canvas/serialize`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(document),
    });
    if (!res.ok) throw new Error("Failed to serialize graph");
    return res.text();
  },
  generateArchitecture: async (prompt: string) => {
    const res = await fetch(`${API_BASE}/canvas/generate`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ prompt }),
    });
    const data = await res.json();
    if (!res.ok) {
      // data may contain { errors: string[] } from ArchParseResult
      const msg =
        data?.errors?.[0] ?? data?.error ?? "Failed to generate architecture";
      throw new Error(msg);
    }
    return data as {
      success: boolean;
      document: { graph: { nodes: unknown[]; edges: unknown[] } } | null;
      errors: string[];
    };
  },
  // Mission Control
  missionControl: {
    getStats: async (projectId: string) => {
      const res = await fetch(`${API_BASE}/missioncontrol/stats/${projectId}`);
      if (!res.ok) throw new Error("Failed to fetch stats");
      return res.json();
    },
    getPipelines: async (projectId: string) => {
      const res = await fetch(
        `${API_BASE}/missioncontrol/pipelines/${projectId}`,
      );
      if (!res.ok) throw new Error("Failed to fetch pipelines");
      return res.json();
    },
    queuePipeline: async (projectId: string, name: string) => {
      const res = await fetch(
        `${API_BASE}/missioncontrol/pipelines/${projectId}?name=${encodeURIComponent(name)}`,
        { method: "POST" },
      );
      if (!res.ok) throw new Error("Failed to queue pipeline");
      return res.json();
    },
  },
};

// --- SignalR Hub Client ---

class OrchestratorHubClient {
  private connection: HubConnection | null = null;

  public async connect() {
    if (typeof window === "undefined") return; // SSR check
    if (!this.connection) {
      this.connection = new HubConnectionBuilder()
        .withUrl(`${HUB_BASE}/orchestrator`)
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();
    }
    if (this.connection.state === "Disconnected") {
      await this.connection.start();
    }
  }

  public onStateUpdate(callback: (state: string, message: string) => void) {
    this.connection?.on("ReceiveStateUpdate", callback);
  }

  public offStateUpdate(callback: (state: string, message: string) => void) {
    this.connection?.off("ReceiveStateUpdate", callback);
  }
}

class AgentHubClient {
  private connection: HubConnection | null = null;

  public async connect() {
    if (typeof window === "undefined") return; // SSR check
    if (!this.connection) {
      this.connection = new HubConnectionBuilder()
        .withUrl(`${HUB_BASE}/agent`)
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();
    }
    if (this.connection.state === "Disconnected") {
      await this.connection.start();
    }
  }

  public onAgentMessage(callback: (message: unknown) => void) {
    this.connection?.on("ReceiveAgentMessage", callback);
  }

  public offAgentMessage(callback: (message: unknown) => void) {
    this.connection?.off("ReceiveAgentMessage", callback);
  }
}

export const orchestratorHub = new OrchestratorHubClient();
export const agentHub = new AgentHubClient();
