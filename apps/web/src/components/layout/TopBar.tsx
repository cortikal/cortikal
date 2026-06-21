"use client";

import React from "react";

interface TopBarProps {
  breadcrumbs?: string[];
}

export default function TopBar({ breadcrumbs = ["Cortikal"] }: TopBarProps) {
  return (
    <header className="topbar">
      <div className="topbar-breadcrumb">
        {breadcrumbs.map((crumb, i) => (
          <React.Fragment key={i}>
            {i > 0 && <span className="topbar-breadcrumb-separator">/</span>}
            <span
              className={
                i === breadcrumbs.length - 1 ? "topbar-breadcrumb-current" : ""
              }
            >
              {crumb}
            </span>
          </React.Fragment>
        ))}
      </div>
      <div className="topbar-actions">
        <button className="topbar-btn" id="btn-notifications" title="Notifications">
          🔔
        </button>
        <button className="topbar-btn" id="btn-settings" title="Settings">
          ⚙️
        </button>
        <button className="topbar-btn" id="btn-theme" title="Toggle Theme">
          🌙
        </button>
      </div>
    </header>
  );
}
