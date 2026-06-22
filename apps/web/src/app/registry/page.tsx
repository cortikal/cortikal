"use client";

import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import AppShell from "../../components/layout/AppShell";
import { ApiClient } from "../../lib/api";
import styles from "../page.module.css";

interface TemplateSummary {
  id: string;
  name: string;
  description: string;
  hasPreview: boolean;
}

export default function RegistryPage() {
  const router = useRouter();
  const [templates, setTemplates] = useState<TemplateSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    ApiClient.getTemplates()
      .then((data) => {
        setTemplates(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Failed to load templates", err);
        setError("Failed to connect to the backend API to load registry templates.");
        setLoading(false);
      });
  }, []);

  const handleTemplateClick = (id: string) => {
    // Navigate to canvas with template query param
    router.push(`/canvas?template=${id}`);
  };

  return (
    <AppShell>
      <div className={styles.spark} style={{ padding: "var(--space-6)" }}>
        <div className={styles.backgroundGlow} />
        <div style={{ position: "relative", zIndex: 1, maxWidth: 1200, margin: "0 auto" }}>
          
          <div style={{ marginBottom: "var(--space-6)" }}>
            <h1 style={{ fontSize: "2.5rem", marginBottom: "var(--space-2)" }}>Architecture Registry</h1>
            <p style={{ color: "var(--color-text-secondary)", fontSize: "var(--text-lg)" }}>
              Browse and import open-source architecture templates.
            </p>
          </div>

          {loading && (
            <div style={{ textAlign: "center", padding: "var(--space-8)", color: "var(--color-text-muted)" }}>
              Loading templates...
            </div>
          )}

          {error && (
            <div style={{ background: "rgba(239, 68, 68, 0.1)", color: "#fca5a5", padding: "var(--space-4)", borderRadius: "var(--radius-md)", border: "1px solid rgba(239, 68, 68, 0.2)" }}>
              {error}
            </div>
          )}

          {!loading && !error && templates.length === 0 && (
            <div style={{ textAlign: "center", padding: "var(--space-8)", color: "var(--color-text-muted)" }}>
              No templates found in the registry.
            </div>
          )}

          <div className={styles.templateGrid}>
            {templates.map((template, index) => (
              <button
                key={template.id}
                className={styles.templateCard}
                onClick={() => handleTemplateClick(template.id)}
                style={{
                  animationDelay: `${index * 60}ms`,
                  textAlign: "left",
                }}
              >
                <div className={styles.templateCardHeader}>
                  <span className={styles.templateIcon}>📦</span>
                  <span
                    className={styles.templateComplexity}
                    style={{
                      color: "var(--color-accent)",
                      borderColor: "var(--color-accent)",
                    }}
                  >
                    Template
                  </span>
                </div>
                <h3 className={styles.templateTitle}>{template.name}</h3>
                <p className={styles.templateDescription}>
                  {template.description}
                </p>
                <div className={styles.templateTags}>
                  <span className={styles.templateTag}>arch.md</span>
                </div>
              </button>
            ))}
          </div>
        </div>
      </div>
    </AppShell>
  );
}
