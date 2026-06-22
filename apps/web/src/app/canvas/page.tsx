"use client";

import dynamic from "next/dynamic";
import AppShell from "../../components/layout/AppShell";

// React Flow requires browser APIs — disable SSR for the editor
const CanvasEditor = dynamic(
  () => import("../../components/canvas/CanvasEditor"),
  { ssr: false }
);

export default function CanvasPage() {
  return (
    <AppShell>
      <CanvasEditor />
    </AppShell>
  );
}
