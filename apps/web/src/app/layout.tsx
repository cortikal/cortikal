import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Cortikal — From Prompt to Production. Visually.",
  description:
    "An open-source, node-based Agentic Software Engineering platform for architecting, coding, and deploying full-stack software systems.",
  keywords: [
    "AI",
    "software engineering",
    "node-based",
    "architecture",
    "code generation",
    "agentic",
    "visual programming",
  ],
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" data-theme="dark">
      <body>{children}</body>
    </html>
  );
}
