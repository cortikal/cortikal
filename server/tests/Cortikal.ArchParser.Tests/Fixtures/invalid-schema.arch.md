---
name: "Invalid Schema"
author: "test"
version: "1.0.0"
tags: [test]
complexity: "simple"
description: "Test with invalid edge references."
---

```arch
nodes:
  - id: node-a
    type: react-app
    category: frontend
    label: "Node A"
    position: { x: 0, y: 0 }
    outputs:
      - id: out-1
        label: "Output"
        direction: output
        dataType: http
        required: true

  - id: node-b
    type: dotnet-api
    category: backend
    label: "Node B"
    position: { x: 200, y: 0 }
    inputs:
      - id: in-1
        label: "Input"
        direction: input
        dataType: sql
        required: true

edges:
  - id: e1
    sourceNodeId: node-a
    sourcePortId: out-1
    targetNodeId: node-nonexistent
    targetPortId: in-1
    dataType: http
    edgeType: dataflow

  - id: e2
    sourceNodeId: node-a
    sourcePortId: out-1
    targetNodeId: node-b
    targetPortId: in-1
    dataType: http
    edgeType: dataflow

  - id: e1
    sourceNodeId: node-a
    sourcePortId: out-1
    targetNodeId: node-a
    targetPortId: out-1
    dataType: http
    edgeType: dataflow
```
