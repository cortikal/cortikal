---
name: Basic Web Application
author: cortikal
version: 1.0.0
tags: [react, dotnet, postgresql, redis]
complexity: simple
description: A standard full-stack web application with React frontend, .NET API backend, PostgreSQL database, and Redis cache.
createdAt: "2025-01-01T00:00:00Z"
---

# Basic Web Application

A production-ready web application starter architecture featuring a React
frontend communicating with a .NET 10 REST API, backed by PostgreSQL for
persistent storage and Redis for caching and session management.

```arch
nodes:
  - id: frontend
    type: react-app
    category: frontend
    label: "React Frontend"
    position: { x: 100, y: 250 }
    config:
      framework: nextjs
      port: 3000
    inputs: []
    outputs:
      - id: http-out
        label: "HTTP Requests"
        direction: output
        dataType: http
        required: true

  - id: api
    type: dotnet-api
    category: backend
    label: ".NET API"
    position: { x: 400, y: 250 }
    config:
      framework: aspnet
      port: 5000
      auth: jwt
    inputs:
      - id: http-in
        label: "HTTP Requests"
        direction: input
        dataType: http
        required: true
    outputs:
      - id: sql-out
        label: "SQL Queries"
        direction: output
        dataType: sql
        required: true
      - id: cache-out
        label: "Cache Operations"
        direction: output
        dataType: json
        required: false

  - id: database
    type: postgresql
    category: database
    label: "PostgreSQL"
    position: { x: 700, y: 200 }
    config:
      version: "16"
      port: 5432
    inputs:
      - id: sql-in
        label: "SQL Queries"
        direction: input
        dataType: sql
        required: true
    outputs: []

  - id: cache
    type: redis
    category: database
    label: "Redis Cache"
    position: { x: 700, y: 350 }
    config:
      version: "7"
      port: 6379
    inputs:
      - id: cache-in
        label: "Cache Operations"
        direction: input
        dataType: json
        required: true
    outputs: []

edges:
  - id: e1
    sourceNodeId: frontend
    sourcePortId: http-out
    targetNodeId: api
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "REST API Calls"

  - id: e2
    sourceNodeId: api
    sourcePortId: sql-out
    targetNodeId: database
    targetPortId: sql-in
    dataType: sql
    edgeType: dataflow
    label: "Database Queries"

  - id: e3
    sourceNodeId: api
    sourcePortId: cache-out
    targetNodeId: cache
    targetPortId: cache-in
    dataType: json
    edgeType: dataflow
    label: "Cache Read/Write"
```
