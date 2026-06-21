"use client";

import React from "react";

export default function StatusBar() {
  return (
    <footer className="statusbar">
      <div className="statusbar-left">
        <div className="statusbar-indicator">
          <span className="statusbar-dot" />
          <span>Ready</span>
        </div>
        <span>v0.1.0-alpha</span>
      </div>
      <div className="statusbar-right">
        <span>Local Mode</span>
        <span>No project loaded</span>
      </div>
    </footer>
  );
}
