"use client";

import React, { useCallback } from "react";
import { useCanvasStore, type NodeCategory, type PortData } from "../../stores/canvasStore";
import styles from "./NodePalette.module.css";

interface NodeTemplate {
  type: string;
  category: NodeCategory;
  label: string;
  icon: string;
  inputs: PortData[];
  outputs: PortData[];
}

const nodeTemplates: Record<string, NodeTemplate[]> = {
  frontend: [
    {
      type: "react-app",
      category: "frontend",
      label: "React App",
      icon: "⚛️",
      inputs: [],
      outputs: [
        { id: "http-out", label: "HTTP Requests", direction: "output", dataType: "http", required: true },
      ],
    },
    {
      type: "vue-app",
      category: "frontend",
      label: "Vue App",
      icon: "💚",
      inputs: [],
      outputs: [
        { id: "http-out", label: "HTTP Requests", direction: "output", dataType: "http", required: true },
      ],
    },
    {
      type: "static-site",
      category: "frontend",
      label: "Static Site",
      icon: "📄",
      inputs: [],
      outputs: [
        { id: "http-out", label: "HTTP Requests", direction: "output", dataType: "http", required: false },
      ],
    },
  ],
  backend: [
    {
      type: "dotnet-api",
      category: "backend",
      label: ".NET API",
      icon: "🟣",
      inputs: [
        { id: "http-in", label: "HTTP Requests", direction: "input", dataType: "http", required: true },
      ],
      outputs: [
        { id: "db-out", label: "DB Queries", direction: "output", dataType: "sql", required: false },
        { id: "http-out", label: "HTTP Out", direction: "output", dataType: "http", required: false },
      ],
    },
    {
      type: "nodejs-api",
      category: "backend",
      label: "Node.js API",
      icon: "🟢",
      inputs: [
        { id: "http-in", label: "HTTP Requests", direction: "input", dataType: "http", required: true },
      ],
      outputs: [
        { id: "db-out", label: "DB Queries", direction: "output", dataType: "json", required: false },
      ],
    },
    {
      type: "python-api",
      category: "backend",
      label: "Python API",
      icon: "🐍",
      inputs: [
        { id: "http-in", label: "HTTP Requests", direction: "input", dataType: "http", required: true },
      ],
      outputs: [
        { id: "db-out", label: "DB Queries", direction: "output", dataType: "sql", required: false },
      ],
    },
    {
      type: "go-service",
      category: "backend",
      label: "Go Service",
      icon: "🔵",
      inputs: [
        { id: "http-in", label: "HTTP Requests", direction: "input", dataType: "http", required: true },
      ],
      outputs: [
        { id: "stream-out", label: "Stream Out", direction: "output", dataType: "stream", required: false },
      ],
    },
  ],
  database: [
    {
      type: "postgresql",
      category: "database",
      label: "PostgreSQL",
      icon: "🐘",
      inputs: [
        { id: "sql-in", label: "SQL Queries", direction: "input", dataType: "sql", required: true },
      ],
      outputs: [],
    },
    {
      type: "mongodb",
      category: "database",
      label: "MongoDB",
      icon: "🍃",
      inputs: [
        { id: "json-in", label: "Documents", direction: "input", dataType: "json", required: true },
      ],
      outputs: [],
    },
    {
      type: "redis",
      category: "database",
      label: "Redis",
      icon: "🔴",
      inputs: [
        { id: "cache-in", label: "Cache Ops", direction: "input", dataType: "json", required: true },
      ],
      outputs: [],
    },
    {
      type: "elasticsearch",
      category: "database",
      label: "Elasticsearch",
      icon: "🔍",
      inputs: [
        { id: "search-in", label: "Search Queries", direction: "input", dataType: "json", required: true },
      ],
      outputs: [],
    },
  ],
  infrastructure: [
    {
      type: "nginx",
      category: "infrastructure",
      label: "Nginx / LB",
      icon: "🔀",
      inputs: [
        { id: "http-in", label: "Traffic", direction: "input", dataType: "http", required: true },
      ],
      outputs: [
        { id: "http-out", label: "Proxied", direction: "output", dataType: "http", required: true },
      ],
    },
    {
      type: "docker",
      category: "infrastructure",
      label: "Docker",
      icon: "🐳",
      inputs: [],
      outputs: [],
    },
    {
      type: "message-queue",
      category: "infrastructure",
      label: "Message Queue",
      icon: "📨",
      inputs: [
        { id: "msg-in", label: "Messages", direction: "input", dataType: "stream", required: true },
      ],
      outputs: [
        { id: "msg-out", label: "Messages", direction: "output", dataType: "stream", required: true },
      ],
    },
  ],
  ai: [
    {
      type: "openai",
      category: "ai",
      label: "OpenAI",
      icon: "🤖",
      inputs: [
        { id: "prompt-in", label: "Prompts", direction: "input", dataType: "json", required: true },
      ],
      outputs: [
        { id: "response-out", label: "Responses", direction: "output", dataType: "json", required: true },
      ],
    },
    {
      type: "vector-db",
      category: "ai",
      label: "Vector DB",
      icon: "🧬",
      inputs: [
        { id: "embed-in", label: "Embeddings", direction: "input", dataType: "json", required: true },
      ],
      outputs: [
        { id: "results-out", label: "Results", direction: "output", dataType: "json", required: true },
      ],
    },
  ],
  integration: [
    {
      type: "stripe",
      category: "integration",
      label: "Stripe",
      icon: "💳",
      inputs: [
        { id: "api-in", label: "API Calls", direction: "input", dataType: "http", required: true },
      ],
      outputs: [],
    },
    {
      type: "auth0",
      category: "integration",
      label: "Auth0",
      icon: "🔐",
      inputs: [
        { id: "auth-in", label: "Auth Requests", direction: "input", dataType: "http", required: true },
      ],
      outputs: [],
    },
    {
      type: "sendgrid",
      category: "integration",
      label: "SendGrid",
      icon: "📧",
      inputs: [
        { id: "email-in", label: "Email Requests", direction: "input", dataType: "json", required: true },
      ],
      outputs: [],
    },
  ],
};

const categoryLabels: Record<string, string> = {
  frontend: "Frontend",
  backend: "Backend",
  database: "Database",
  infrastructure: "Infrastructure",
  ai: "AI / ML",
  integration: "Integrations",
};

const categoryColors: Record<string, string> = {
  frontend: "var(--color-node-frontend)",
  backend: "var(--color-node-backend)",
  database: "var(--color-node-database)",
  infrastructure: "var(--color-node-infra)",
  ai: "var(--color-node-ai)",
  integration: "var(--color-accent)",
};

export default function NodePalette() {
  const addNode = useCanvasStore((s) => s.addNode);
  const isPaletteOpen = useCanvasStore((s) => s.isPaletteOpen);

  const handleAddNode = useCallback(
    (template: NodeTemplate) => {
      // Place new nodes with some randomization around center
      const x = 300 + Math.random() * 400;
      const y = 150 + Math.random() * 300;
      addNode(
        template.type,
        template.category,
        template.label,
        { x, y },
        template.inputs.map((p, i) => ({ ...p, id: `${p.id}-${Date.now()}-${i}` })),
        template.outputs.map((p, i) => ({ ...p, id: `${p.id}-${Date.now()}-${i}` }))
      );
    },
    [addNode]
  );

  if (!isPaletteOpen) return null;

  return (
    <div className={styles.palette} id="node-palette">
      <div className={styles.header}>
        <span className={styles.headerIcon}>🧩</span>
        <span className={styles.headerTitle}>Components</span>
      </div>

      <div className={styles.categories}>
        {Object.entries(nodeTemplates).map(([category, templates]) => (
          <div key={category} className={styles.category}>
            <div className={styles.categoryHeader}>
              <span
                className={styles.categoryDot}
                style={{ background: categoryColors[category] }}
              />
              <span className={styles.categoryLabel}>
                {categoryLabels[category]}
              </span>
            </div>
            <div className={styles.templateList}>
              {templates.map((template) => (
                <button
                  key={template.type}
                  className={styles.templateItem}
                  onClick={() => handleAddNode(template)}
                  id={`palette-${template.type}`}
                  title={`Add ${template.label}`}
                >
                  <span className={styles.templateIcon}>{template.icon}</span>
                  <span className={styles.templateName}>{template.label}</span>
                </button>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
