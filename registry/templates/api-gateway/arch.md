---
name: "API Gateway + Microservices"
author: "cortikal"
version: "1.0.0"
tags: [gateway, microservices, docker, kubernetes, rabbitmq, dotnet]
complexity: "moderate"
description: "Microservice orchestration pattern with an API gateway, backend services, per-service databases, and async messaging."
createdAt: "2025-01-15T00:00:00Z"
updatedAt: "2025-06-20T12:00:00Z"
---

# API Gateway + Microservices Architecture

A microservice orchestration pattern where an API gateway routes requests
to domain-specific backend services, each with its own database. Services
communicate asynchronously via a message queue.

```arch
nodes:
  - id: client-app
    type: react-app
    category: frontend
    label: "Client Application"
    position: { x: 100, y: 260 }
    outputs:
      - id: api-out
        label: "API Requests"
        direction: output
        dataType: http
        required: true

  - id: api-gateway
    type: nginx
    category: infrastructure
    label: "API Gateway"
    position: { x: 420, y: 260 }
    config:
      rateLimit: true
      loadBalancing: round-robin
    inputs:
      - id: http-in
        label: "Incoming Requests"
        direction: input
        dataType: http
        required: true
    outputs:
      - id: user-out
        label: "User Routes"
        direction: output
        dataType: http
        required: true
      - id: product-out
        label: "Product Routes"
        direction: output
        dataType: http
        required: true
      - id: order-out
        label: "Order Routes"
        direction: output
        dataType: http
        required: true

  - id: user-service
    type: dotnet-api
    category: backend
    label: "User Service"
    position: { x: 740, y: 100 }
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
      - id: event-out
        label: "Events"
        direction: output
        dataType: event
        required: false

  - id: product-service
    type: dotnet-api
    category: backend
    label: "Product Service"
    position: { x: 740, y: 300 }
    inputs:
      - id: http-in
        label: "HTTP Requests"
        direction: input
        dataType: http
        required: true
      - id: event-in
        label: "Events"
        direction: input
        dataType: event
        required: false
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
    position: { x: 740, y: 500 }
    inputs:
      - id: http-in
        label: "HTTP Requests"
        direction: input
        dataType: http
        required: true
      - id: event-in
        label: "Events"
        direction: input
        dataType: event
        required: false
    outputs:
      - id: db-out
        label: "DB Queries"
        direction: output
        dataType: sql
        required: true
      - id: event-out
        label: "Events"
        direction: output
        dataType: event
        required: false

  - id: user-db
    type: postgresql
    category: database
    label: "User Database"
    position: { x: 1060, y: 100 }
    inputs:
      - id: sql-in
        label: "SQL Queries"
        direction: input
        dataType: sql
        required: true

  - id: product-db
    type: postgresql
    category: database
    label: "Product Database"
    position: { x: 1060, y: 300 }
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
    position: { x: 1060, y: 500 }
    inputs:
      - id: sql-in
        label: "SQL Queries"
        direction: input
        dataType: sql
        required: true

  - id: message-queue
    type: rabbitmq
    category: integration
    label: "RabbitMQ"
    position: { x: 1060, y: 700 }
    inputs:
      - id: event-in-user
        label: "User Events"
        direction: input
        dataType: event
        required: false
      - id: event-in-order
        label: "Order Events"
        direction: input
        dataType: event
        required: false
    outputs:
      - id: event-out-product
        label: "To Product Service"
        direction: output
        dataType: event
        required: false
      - id: event-out-order
        label: "To Order Service"
        direction: output
        dataType: event
        required: false

edges:
  - id: e1
    sourceNodeId: client-app
    sourcePortId: api-out
    targetNodeId: api-gateway
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "Client Requests"

  - id: e2
    sourceNodeId: api-gateway
    sourcePortId: user-out
    targetNodeId: user-service
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "User API"

  - id: e3
    sourceNodeId: api-gateway
    sourcePortId: product-out
    targetNodeId: product-service
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "Product API"

  - id: e4
    sourceNodeId: api-gateway
    sourcePortId: order-out
    targetNodeId: order-service
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "Order API"

  - id: e5
    sourceNodeId: user-service
    sourcePortId: db-out
    targetNodeId: user-db
    targetPortId: sql-in
    dataType: sql
    edgeType: dataflow

  - id: e6
    sourceNodeId: product-service
    sourcePortId: db-out
    targetNodeId: product-db
    targetPortId: sql-in
    dataType: sql
    edgeType: dataflow

  - id: e7
    sourceNodeId: order-service
    sourcePortId: db-out
    targetNodeId: order-db
    targetPortId: sql-in
    dataType: sql
    edgeType: dataflow

  - id: e8
    sourceNodeId: user-service
    sourcePortId: event-out
    targetNodeId: message-queue
    targetPortId: event-in-user
    dataType: event
    edgeType: dataflow
    label: "User Events"

  - id: e9
    sourceNodeId: order-service
    sourcePortId: event-out
    targetNodeId: message-queue
    targetPortId: event-in-order
    dataType: event
    edgeType: dataflow
    label: "Order Events"

  - id: e10
    sourceNodeId: message-queue
    sourcePortId: event-out-product
    targetNodeId: product-service
    targetPortId: event-in
    dataType: event
    edgeType: dataflow
    label: "Inventory Sync"

  - id: e11
    sourceNodeId: message-queue
    sourcePortId: event-out-order
    targetNodeId: order-service
    targetPortId: event-in
    dataType: event
    edgeType: dataflow
    label: "Order Updates"
```
