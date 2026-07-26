---
name: "ML Pipeline Dashboard"
author: "cortikal"
version: "1.0.0"
tags: [python, machine-learning, docker, monitoring, fastapi, s3]
complexity: "enterprise"
description: "End-to-end ML pipeline from data ingestion through model training to deployment, with a monitoring dashboard."
createdAt: "2025-02-01T00:00:00Z"
updatedAt: "2025-07-01T12:00:00Z"
---

# ML Pipeline Architecture

An end-to-end machine learning pipeline covering data ingestion,
preprocessing, model training, model serving, and a monitoring dashboard.

```arch
nodes:
  - id: dashboard
    type: react-app
    category: frontend
    label: "ML Dashboard"
    position: { x: 100, y: 300 }
    config:
      framework: react
      port: 3000
    inputs:
      - id: metrics-in
        label: "Metrics Feed"
        direction: input
        dataType: http
        required: false
    outputs:
      - id: api-out
        label: "API Requests"
        direction: output
        dataType: http
        required: true

  - id: data-ingestion
    type: node-api
    category: backend
    label: "Data Ingestion Service"
    position: { x: 420, y: 100 }
    config:
      runtime: python
      framework: fastapi
    inputs:
      - id: http-in
        label: "Trigger / Upload"
        direction: input
        dataType: http
        required: true
    outputs:
      - id: stream-out
        label: "Raw Data Stream"
        direction: output
        dataType: stream
        required: true
      - id: storage-out
        label: "Raw Storage"
        direction: output
        dataType: json
        required: true

  - id: preprocessor
    type: node-api
    category: backend
    label: "Preprocessing Pipeline"
    position: { x: 740, y: 100 }
    config:
      runtime: python
    inputs:
      - id: stream-in
        label: "Raw Data"
        direction: input
        dataType: stream
        required: true
    outputs:
      - id: stream-out
        label: "Processed Data"
        direction: output
        dataType: stream
        required: true
      - id: storage-out
        label: "Processed Storage"
        direction: output
        dataType: json
        required: true

  - id: model-trainer
    type: node-api
    category: ai
    label: "Model Training"
    position: { x: 1060, y: 100 }
    config:
      runtime: python
      gpu: true
    inputs:
      - id: data-in
        label: "Training Data"
        direction: input
        dataType: stream
        required: true
    outputs:
      - id: model-out
        label: "Model Artifact"
        direction: output
        dataType: json
        required: true
      - id: metrics-out
        label: "Training Metrics"
        direction: output
        dataType: json
        required: true

  - id: model-server
    type: node-api
    category: ai
    label: "Model Serving (FastAPI)"
    position: { x: 1060, y: 400 }
    config:
      runtime: python
      framework: fastapi
    inputs:
      - id: model-in
        label: "Model Artifact"
        direction: input
        dataType: json
        required: true
      - id: http-in
        label: "Inference Requests"
        direction: input
        dataType: http
        required: true
    outputs:
      - id: metrics-out
        label: "Inference Metrics"
        direction: output
        dataType: json
        required: false

  - id: metadata-db
    type: postgresql
    category: database
    label: "Metadata Store"
    position: { x: 420, y: 500 }
    inputs:
      - id: sql-in
        label: "Metadata Queries"
        direction: input
        dataType: sql
        required: true

  - id: artifact-store
    type: s3
    category: infrastructure
    label: "Artifact Storage (S3)"
    position: { x: 740, y: 500 }
    inputs:
      - id: raw-in
        label: "Raw Data"
        direction: input
        dataType: json
        required: true
      - id: processed-in
        label: "Processed Data"
        direction: input
        dataType: json
        required: true
      - id: model-in
        label: "Model Files"
        direction: input
        dataType: json
        required: true

  - id: monitoring
    type: docker
    category: infrastructure
    label: "Monitoring (Prometheus)"
    position: { x: 1060, y: 600 }
    inputs:
      - id: metrics-train
        label: "Training Metrics"
        direction: input
        dataType: json
        required: false
      - id: metrics-serve
        label: "Inference Metrics"
        direction: input
        dataType: json
        required: false
    outputs:
      - id: http-out
        label: "Metrics Dashboard"
        direction: output
        dataType: http
        required: true

edges:
  - id: e1
    sourceNodeId: dashboard
    sourcePortId: api-out
    targetNodeId: data-ingestion
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "Upload / Trigger"

  - id: e2
    sourceNodeId: data-ingestion
    sourcePortId: stream-out
    targetNodeId: preprocessor
    targetPortId: stream-in
    dataType: stream
    edgeType: dataflow
    label: "Raw Data"

  - id: e3
    sourceNodeId: preprocessor
    sourcePortId: stream-out
    targetNodeId: model-trainer
    targetPortId: data-in
    dataType: stream
    edgeType: dataflow
    label: "Processed Data"

  - id: e4
    sourceNodeId: model-trainer
    sourcePortId: model-out
    targetNodeId: model-server
    targetPortId: model-in
    dataType: json
    edgeType: dataflow
    label: "Deploy Model"

  - id: e5
    sourceNodeId: model-trainer
    sourcePortId: model-out
    targetNodeId: artifact-store
    targetPortId: model-in
    dataType: json
    edgeType: dataflow
    label: "Save Model"

  - id: e6
    sourceNodeId: data-ingestion
    sourcePortId: storage-out
    targetNodeId: artifact-store
    targetPortId: raw-in
    dataType: json
    edgeType: dataflow
    label: "Store Raw Data"

  - id: e7
    sourceNodeId: preprocessor
    sourcePortId: storage-out
    targetNodeId: artifact-store
    targetPortId: processed-in
    dataType: json
    edgeType: dataflow
    label: "Store Processed"

  - id: e8
    sourceNodeId: model-trainer
    sourcePortId: metrics-out
    targetNodeId: monitoring
    targetPortId: metrics-train
    dataType: json
    edgeType: dataflow
    label: "Training Metrics"

  - id: e9
    sourceNodeId: model-server
    sourcePortId: metrics-out
    targetNodeId: monitoring
    targetPortId: metrics-serve
    dataType: json
    edgeType: dataflow
    label: "Serving Metrics"

  - id: e10
    sourceNodeId: dashboard
    sourcePortId: api-out
    targetNodeId: model-server
    targetPortId: http-in
    dataType: http
    edgeType: dataflow
    label: "Inference Requests"

  - id: e11
    sourceNodeId: monitoring
    sourcePortId: http-out
    targetNodeId: dashboard
    targetPortId: metrics-in
    dataType: http
    edgeType: dependency
    label: "Metrics Feed"
```
