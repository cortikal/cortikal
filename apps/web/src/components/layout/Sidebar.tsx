"use client";

import React, { useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";

interface NavItem {
  id: string;
  label: string;
  href: string;
  icon: string;
  badge?: string;
}

const navSections: { label: string; items: NavItem[] }[] = [
  {
    label: "Build",
    items: [
      { id: "spark", label: "The Spark", href: "/", icon: "⚡" },
      { id: "canvas", label: "The Canvas", href: "/canvas", icon: "🎨" },
      { id: "registry", label: "Registry", href: "/registry", icon: "📦" },
    ],
  },
  {
    label: "Execute",
    items: [
      { id: "swarm", label: "Agent Swarm", href: "/swarm", icon: "🤖", badge: "AI" },
      { id: "mission", label: "Mission Control", href: "/mission-control", icon: "🚀" },
    ],
  },
];

export default function Sidebar() {
  const pathname = usePathname();
  const [collapsed, setCollapsed] = useState(false);

  return (
    <aside className={`sidebar ${collapsed ? "collapsed" : ""}`}>
      {/* Logo */}
      <div className="sidebar-logo">
        <div className="sidebar-logo-icon">C</div>
        {!collapsed && (
          <>
            <span className="sidebar-logo-text">Cortikal</span>
            <span className="sidebar-logo-badge">Alpha</span>
          </>
        )}
      </div>

      {/* Navigation */}
      <nav className="sidebar-nav">
        {navSections.map((section) => (
          <React.Fragment key={section.label}>
            {!collapsed && (
              <div className="sidebar-section-label">{section.label}</div>
            )}
            {section.items.map((item) => {
              const isActive =
                item.href === "/"
                  ? pathname === "/"
                  : pathname.startsWith(item.href);

              return (
                <Link
                  key={item.id}
                  href={item.href}
                  className={`nav-item ${isActive ? "active" : ""}`}
                  id={`nav-${item.id}`}
                >
                  <span className="nav-item-icon">{item.icon}</span>
                  {!collapsed && (
                    <>
                      <span>{item.label}</span>
                      {item.badge && (
                        <span className="nav-item-badge">{item.badge}</span>
                      )}
                    </>
                  )}
                </Link>
              );
            })}
          </React.Fragment>
        ))}
      </nav>

      {/* Footer */}
      <div className="sidebar-footer">
        <button
          className="nav-item"
          onClick={() => setCollapsed(!collapsed)}
          id="sidebar-toggle"
          style={{ width: "100%" }}
        >
          <span className="nav-item-icon">{collapsed ? "→" : "←"}</span>
          {!collapsed && <span>Collapse</span>}
        </button>
        <div className="sidebar-user">
          <div className="sidebar-user-avatar">U</div>
          {!collapsed && (
            <div className="sidebar-user-info">
              <div className="sidebar-user-name">Local User</div>
              <div className="sidebar-user-role">Developer</div>
            </div>
          )}
        </div>
      </div>
    </aside>
  );
}
