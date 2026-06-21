---
name: "Simple Web App"
author: "cortikal-test"
version: "1.0.0"
tags: [react, nodejs, postgresql]
complexity: "simple"
description: "A simple web application for testing."
---

# Simple Web Application

A basic full-stack web application.

```arch
nodes:
  - id: frontend
    type: react-app
    category: frontend
    label: "React Frontend"
    position: { x: 100, y: 200 }
    outputs:
      - id: api-out
        label: "API Requests"
        direction: output
        dataType: http
        required: true

  - id: api
    type: nodejs-api
    category: backend
    label: "Node.js API"
    position: { x: 400, y: 200 }
    inputs:
      - id: http-in
        label: "HTTP Requests"
        direction: input
        dataType: http
        required: true
    outputs:
      - id: db-out
        label: "DB Queries"
        direction: output
        dataType: sql
        required: true

  - id: database
    type: postgresql
    category: database
    label: "PostgreSQL"
    position: { x: 700, y: 200 }
    inputs:
      - id: sql-in
        label: "SQL Queries"
        direction: input
        dataType: sql
        required: true

edges:
  - id: e1
    sourceNodeId: frontend
    sourcePortId: api-out
    targetNodeId: api
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "REST API"

  - id: e2
    sourceNodeId: api
    sourcePortId: db-out
    targetNodeId: database
    targetPortId: sql-in
    dataType: sql
    edgeType: dataflow
    label: "Database Queries"
```
