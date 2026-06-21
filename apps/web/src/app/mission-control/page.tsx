import AppShell from "../../components/layout/AppShell";

export default function MissionControlPage() {
  return (
    <AppShell>
      <div style={{
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        height: "100%",
        color: "var(--color-text-tertiary)",
        fontSize: "var(--text-lg)",
      }}>
        <div style={{ textAlign: "center" }}>
          <div style={{ fontSize: "3rem", marginBottom: "var(--space-4)" }}>🚀</div>
          <h2 style={{ color: "var(--color-text-primary)", marginBottom: "var(--space-2)" }}>
            Mission Control
          </h2>
          <p>DevOps deployment dashboard — coming in Phase 5</p>
        </div>
      </div>
    </AppShell>
  );
}
