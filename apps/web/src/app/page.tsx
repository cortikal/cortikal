"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import AppShell from "../components/layout/AppShell";
import styles from "./page.module.css";

const quickStartTemplates = [
  {
    id: "web-app",
    title: "Full-Stack Web App",
    description: "React + .NET API + PostgreSQL",
    icon: "🌐",
    complexity: "simple" as const,
    tags: ["react", "dotnet", "postgresql"],
  },
  {
    id: "ecommerce",
    title: "E-Commerce Platform",
    description: "Microservices with payment processing",
    icon: "🛒",
    complexity: "complex" as const,
    tags: ["microservices", "stripe", "redis"],
  },
  {
    id: "realtime-chat",
    title: "Real-Time Chat App",
    description: "WebSocket-based with message persistence",
    icon: "💬",
    complexity: "moderate" as const,
    tags: ["websocket", "mongodb", "react"],
  },
  {
    id: "saas-starter",
    title: "SaaS Starter Kit",
    description: "Multi-tenant with auth & billing",
    icon: "🚀",
    complexity: "complex" as const,
    tags: ["auth", "stripe", "multi-tenant"],
  },
  {
    id: "api-gateway",
    title: "API Gateway + Services",
    description: "Microservice orchestration pattern",
    icon: "🔀",
    complexity: "moderate" as const,
    tags: ["gateway", "docker", "kubernetes"],
  },
  {
    id: "ml-pipeline",
    title: "ML Pipeline Dashboard",
    description: "Data ingestion to model deployment",
    icon: "🧠",
    complexity: "enterprise" as const,
    tags: ["python", "docker", "monitoring"],
  },
];

const complexityColors: Record<string, string> = {
  simple: "var(--color-success)",
  moderate: "var(--color-warning)",
  complex: "var(--color-accent)",
  enterprise: "var(--color-secondary)",
};

export default function SparkPage() {
  const router = useRouter();
  const [prompt, setPrompt] = useState("");

  const handleGenerate = () => {
    if (prompt.trim()) {
      router.push(`/canvas?prompt=${encodeURIComponent(prompt.trim())}`);
    }
  };

  const handleTemplateClick = (id: string) => {
    router.push(`/canvas?template=${id}`);
  };

  return (
    <AppShell>
      <div className={styles.spark}>
        {/* Background Glow */}
        <div className={styles.backgroundGlow} />

        {/* Hero Section */}
        <div className={styles.hero}>
          <div className={styles.heroContent}>
            <h1 className={styles.heroTitle}>
              <span className={styles.heroTitleGradient}>From Prompt</span>
              <br />
              to Production.
            </h1>
            <p className={styles.heroSubtitle}>
              Describe your architecture in plain English or browse community
              templates. Cortikal&apos;s AI agents will build it for you.
            </p>

            {/* Prompt Input */}
            <div className={styles.promptContainer}>
              <div className={styles.promptInputWrapper}>
                <span className={styles.promptIcon}>⚡</span>
                <input
                  id="spark-prompt-input"
                  type="text"
                  className={styles.promptInput}
                  placeholder="Describe your software architecture..."
                  value={prompt}
                  onChange={(e) => setPrompt(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter" && prompt.trim()) {
                      handleGenerate();
                    }
                  }}
                />
                <button
                  className={styles.promptButton}
                  id="spark-generate-btn"
                  disabled={!prompt.trim()}
                  onClick={handleGenerate}
                >
                  Generate
                  <span className={styles.promptButtonIcon}>→</span>
                </button>
              </div>
              <div className={styles.promptHints}>
                <span className={styles.promptHint}>Try:</span>
                <button
                  className={styles.promptSuggestion}
                  onClick={() =>
                    setPrompt("A train ticket booking system with real-time seat selection")
                  }
                >
                  Train ticket booking system
                </button>
                <button
                  className={styles.promptSuggestion}
                  onClick={() =>
                    setPrompt("Real-time collaborative document editor like Google Docs")
                  }
                >
                  Collaborative editor
                </button>
                <button
                  className={styles.promptSuggestion}
                  onClick={() =>
                    setPrompt("IoT dashboard with sensor data visualization and alerts")
                  }
                >
                  IoT dashboard
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Divider */}
        <div className={styles.divider}>
          <span className={styles.dividerText}>or browse templates</span>
        </div>

        {/* Template Grid */}
        <div className={styles.templateSection}>
          <div className={styles.templateGrid}>
            {quickStartTemplates.map((template, index) => (
              <button
                key={template.id}
                className={styles.templateCard}
                id={`template-${template.id}`}
                onClick={() => handleTemplateClick(template.id)}
                style={{
                  animationDelay: `${index * 60}ms`,
                }}
              >
                <div className={styles.templateCardHeader}>
                  <span className={styles.templateIcon}>{template.icon}</span>
                  <span
                    className={styles.templateComplexity}
                    style={{
                      color: complexityColors[template.complexity],
                      borderColor: complexityColors[template.complexity],
                    }}
                  >
                    {template.complexity}
                  </span>
                </div>
                <h3 className={styles.templateTitle}>{template.title}</h3>
                <p className={styles.templateDescription}>
                  {template.description}
                </p>
                <div className={styles.templateTags}>
                  {template.tags.map((tag) => (
                    <span key={tag} className={styles.templateTag}>
                      {tag}
                    </span>
                  ))}
                </div>
              </button>
            ))}
          </div>
        </div>
      </div>
    </AppShell>
  );
}
