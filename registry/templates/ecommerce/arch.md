---
name: "E-Commerce Microservices"
author: "cortikal"
version: "1.0.0"
tags: [ecommerce, microservices, dotnet, react, redis, postgresql]
complexity: "complex"
description: "A microservices-based e-commerce platform with API gateway."
createdAt: "2025-01-01T00:00:00Z"
updatedAt: "2025-06-15T12:00:00Z"
---

# E-Commerce Microservices Architecture

A scalable e-commerce system decomposed into domain-specific microservices.

```arch
nodes:
  - id: storefront
    type: react-app
    category: frontend
    label: "Storefront (Next.js)"
    position: { x: 100, y: 300 }
    config:
      framework: nextjs
      port: 3000
    outputs:
      - id: api-out
        label: "API Calls"
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
      rateLimit: true
    inputs:
      - id: http-in
        label: "HTTP Requests"
        direction: input
        dataType: http
        required: true
    outputs:
      - id: product-out
        label: "Product Requests"
        direction: output
        dataType: http
        required: true
      - id: order-out
        label: "Order Requests"
        direction: output
        dataType: http
        required: true
      - id: cache-out
        label: "Cache Ops"
        direction: output
        dataType: json
        required: false

  - id: product-service
    type: dotnet-api
    category: backend
    label: "Product Service"
    position: { x: 700, y: 200 }
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

  - id: order-service
    type: dotnet-api
    category: backend
    label: "Order Service"
    position: { x: 700, y: 400 }
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

  - id: product-db
    type: postgresql
    category: database
    label: "Product Database"
    position: { x: 1000, y: 200 }
    inputs:
      - id: sql-in
        label: "SQL Queries"
        direction: input
        dataType: sql
        required: true

  - id: order-db
    type: postgresql
    category: database
    label: "Order Database"
    position: { x: 1000, y: 400 }
    inputs:
      - id: sql-in
        label: "SQL Queries"
        direction: input
        dataType: sql
        required: true

  - id: cache
    type: redis
    category: database
    label: "Redis Cache"
    position: { x: 700, y: 500 }
    inputs:
      - id: cache-in
        label: "Cache Operations"
        direction: input
        dataType: json
        required: true

edges:
  - id: e1
    sourceNodeId: storefront
    sourcePortId: api-out
    targetNodeId: api-gateway
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "Client Requests"

  - id: e2
    sourceNodeId: api-gateway
    sourcePortId: product-out
    targetNodeId: product-service
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "Product API"

  - id: e3
    sourceNodeId: api-gateway
    sourcePortId: order-out
    targetNodeId: order-service
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "Order API"

  - id: e4
    sourceNodeId: product-service
    sourcePortId: db-out
    targetNodeId: product-db
    targetPortId: sql-in
    dataType: sql
    edgeType: dataflow

  - id: e5
    sourceNodeId: order-service
    sourcePortId: db-out
    targetNodeId: order-db
    targetPortId: sql-in
    dataType: sql
    edgeType: dataflow

  - id: e6
    sourceNodeId: api-gateway
    sourcePortId: cache-out
    targetNodeId: cache
    targetPortId: cache-in
    dataType: json
    edgeType: dataflow
    label: "Session Cache"
```
