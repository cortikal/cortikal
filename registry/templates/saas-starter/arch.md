---
name: "SaaS Starter Kit"
author: "cortikal"
version: "1.0.0"
tags: [saas, multi-tenant, auth, stripe, react, dotnet, postgresql, redis]
complexity: "complex"
description: "A production-ready SaaS starter with multi-tenancy, authentication, billing, and admin dashboard."
---

# SaaS Starter Kit Architecture

A comprehensive SaaS foundation with tenant isolation, JWT authentication,
Stripe billing integration, background job processing, and an admin panel.

```arch
nodes:
  - id: app-frontend
    type: react-app
    category: frontend
    label: "SaaS Dashboard (Next.js)"
    position: { x: 100, y: 200 }
    config:
      framework: nextjs
      port: 3000
      auth: true
    outputs:
      - id: api-out
        label: "API Calls"
        direction: output
        dataType: http
        required: true

  - id: admin-frontend
    type: react-app
    category: frontend
    label: "Admin Panel (React)"
    position: { x: 100, y: 450 }
    outputs:
      - id: admin-api-out
        label: "Admin API"
        direction: output
        dataType: http
        required: true

  - id: api-gateway
    type: dotnet-api
    category: backend
    label: "API Gateway (.NET)"
    position: { x: 400, y: 300 }
    config:
      auth: jwt
      multiTenant: true
    inputs:
      - id: http-in
        label: "HTTP Requests"
        direction: input
        dataType: http
        required: true
      - id: admin-in
        label: "Admin Requests"
        direction: input
        dataType: http
        required: true
    outputs:
      - id: db-out
        label: "Database"
        direction: output
        dataType: sql
        required: true
      - id: cache-out
        label: "Cache"
        direction: output
        dataType: json
        required: false
      - id: billing-out
        label: "Billing"
        direction: output
        dataType: http
        required: true
      - id: auth-out
        label: "Auth Provider"
        direction: output
        dataType: http
        required: true

  - id: main-db
    type: postgresql
    category: database
    label: "Main Database (PostgreSQL)"
    position: { x: 750, y: 150 }
    config:
      multiTenant: schema-per-tenant
    inputs:
      - id: sql-in
        label: "SQL Queries"
        direction: input
        dataType: sql
        required: true

  - id: session-cache
    type: redis
    category: database
    label: "Session Cache (Redis)"
    position: { x: 750, y: 300 }
    inputs:
      - id: cache-in
        label: "Cache Ops"
        direction: input
        dataType: json
        required: true

  - id: stripe
    type: stripe
    category: integration
    label: "Stripe Billing"
    position: { x: 750, y: 450 }
    inputs:
      - id: billing-in
        label: "Billing API"
        direction: input
        dataType: http
        required: true

  - id: auth-provider
    type: auth0
    category: integration
    label: "Auth0 / Identity"
    position: { x: 750, y: 600 }
    inputs:
      - id: auth-in
        label: "Auth Requests"
        direction: input
        dataType: http
        required: true

edges:
  - id: e1
    sourceNodeId: app-frontend
    sourcePortId: api-out
    targetNodeId: api-gateway
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "User Requests"

  - id: e2
    sourceNodeId: admin-frontend
    sourcePortId: admin-api-out
    targetNodeId: api-gateway
    targetPortId: admin-in
    dataType: http
    edgeType: dataflow
    label: "Admin Requests"

  - id: e3
    sourceNodeId: api-gateway
    sourcePortId: db-out
    targetNodeId: main-db
    targetPortId: sql-in
    dataType: sql
    edgeType: dataflow

  - id: e4
    sourceNodeId: api-gateway
    sourcePortId: cache-out
    targetNodeId: session-cache
    targetPortId: cache-in
    dataType: json
    edgeType: dataflow

  - id: e5
    sourceNodeId: api-gateway
    sourcePortId: billing-out
    targetNodeId: stripe
    targetPortId: billing-in
    dataType: http
    edgeType: dataflow
    label: "Billing API"

  - id: e6
    sourceNodeId: api-gateway
    sourcePortId: auth-out
    targetNodeId: auth-provider
    targetPortId: auth-in
    dataType: http
    edgeType: dataflow
    label: "Authentication"
```
