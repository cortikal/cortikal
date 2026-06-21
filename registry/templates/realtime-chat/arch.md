---
name: "Real-Time Chat Application"
author: "cortikal"
version: "1.0.0"
tags: [chat, websocket, react, nodejs, mongodb, redis]
complexity: "moderate"
description: "A real-time chat application with WebSocket messaging, message persistence, and presence tracking."
---

# Real-Time Chat Architecture

A scalable real-time chat system with WebSocket support, message history,
and online presence tracking via Redis pub/sub.

```arch
nodes:
  - id: chat-ui
    type: react-app
    category: frontend
    label: "Chat Client (React)"
    position: { x: 100, y: 250 }
    config:
      framework: vite
      port: 5173
    outputs:
      - id: ws-out
        label: "WebSocket Messages"
        direction: output
        dataType: websocket
        required: true
      - id: api-out
        label: "REST API"
        direction: output
        dataType: http
        required: true

  - id: chat-server
    type: nodejs-api
    category: backend
    label: "Chat Server (Node.js)"
    position: { x: 450, y: 250 }
    config:
      runtime: nodejs
      wsLibrary: socket.io
    inputs:
      - id: ws-in
        label: "WebSocket Connections"
        direction: input
        dataType: websocket
        required: true
      - id: api-in
        label: "REST Requests"
        direction: input
        dataType: http
        required: true
    outputs:
      - id: db-out
        label: "Message Store"
        direction: output
        dataType: json
        required: true
      - id: cache-out
        label: "Presence & Pub/Sub"
        direction: output
        dataType: json
        required: true

  - id: message-db
    type: mongodb
    category: database
    label: "Message Store (MongoDB)"
    position: { x: 800, y: 150 }
    inputs:
      - id: queries-in
        label: "CRUD Operations"
        direction: input
        dataType: json
        required: true

  - id: presence-cache
    type: redis
    category: database
    label: "Presence & Pub/Sub (Redis)"
    position: { x: 800, y: 350 }
    config:
      features: [pubsub, presence]
    inputs:
      - id: cache-in
        label: "Pub/Sub & Cache"
        direction: input
        dataType: json
        required: true

edges:
  - id: e1
    sourceNodeId: chat-ui
    sourcePortId: ws-out
    targetNodeId: chat-server
    targetPortId: ws-in
    dataType: websocket
    edgeType: dataflow
    label: "Real-Time Messages"

  - id: e2
    sourceNodeId: chat-ui
    sourcePortId: api-out
    targetNodeId: chat-server
    targetPortId: api-in
    dataType: http
    edgeType: dataflow
    label: "History & Auth"

  - id: e3
    sourceNodeId: chat-server
    sourcePortId: db-out
    targetNodeId: message-db
    targetPortId: queries-in
    dataType: json
    edgeType: dataflow
    label: "Persist Messages"

  - id: e4
    sourceNodeId: chat-server
    sourcePortId: cache-out
    targetNodeId: presence-cache
    targetPortId: cache-in
    dataType: json
    edgeType: dataflow
    label: "Online Status"
```
