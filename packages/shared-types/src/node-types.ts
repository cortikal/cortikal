/**
 * Cortikal — Node Category Definitions
 *
 * Defines all supported node categories on the visual canvas.
 * Each category groups related technology nodes together.
 */

/** Top-level node categories displayed in the palette sidebar */
export enum NodeCategory {
  Frontend = "frontend",
  Backend = "backend",
  Database = "database",
  Infrastructure = "infrastructure",
  AI = "ai",
  Integration = "integration",
  Custom = "custom",
}

/** Supported technology stacks per node category */
export enum FrontendTech {
  React = "react",
  NextJS = "nextjs",
  Vue = "vue",
  Angular = "angular",
  Svelte = "svelte",
  Static = "static",
}

export enum BackendTech {
  DotNet = "dotnet",
  NodeJS = "nodejs",
  Python = "python",
  Go = "go",
  Rust = "rust",
  Java = "java",
}

export enum DatabaseTech {
  PostgreSQL = "postgresql",
  MySQL = "mysql",
  MongoDB = "mongodb",
  Redis = "redis",
  SQLite = "sqlite",
  Elasticsearch = "elasticsearch",
}

export enum InfraTech {
  Docker = "docker",
  Kubernetes = "kubernetes",
  Nginx = "nginx",
  AWS = "aws",
  Azure = "azure",
  GCP = "gcp",
}

export enum AITech {
  OpenAI = "openai",
  Anthropic = "anthropic",
  Google = "google",
  LocalLLM = "local_llm",
  VectorDB = "vector_db",
}

/** Port data types for I/O contract enforcement */
export enum PortDataType {
  String = "string",
  Number = "number",
  Boolean = "boolean",
  Object = "object",
  Array = "array",
  Stream = "stream",
  Binary = "binary",
  JSON = "json",
  HTTP = "http",
  WebSocket = "websocket",
  SQL = "sql",
  GraphQL = "graphql",
}

/** Direction of a port on a node */
export enum PortDirection {
  Input = "input",
  Output = "output",
}

/** Defines a single I/O port on a node */
export interface Port {
  /** Unique identifier within the node */
  id: string;
  /** Human-readable label */
  label: string;
  /** Direction: input or output */
  direction: PortDirection;
  /** Data type for contract enforcement */
  dataType: PortDataType;
  /** Whether this port is required for the node to function */
  required: boolean;
  /** Optional description */
  description?: string;
  /** Optional JSON Schema for complex types */
  schema?: Record<string, unknown>;
}

/** Base node definition shared by all node types */
export interface NodeDefinition {
  /** Unique node type identifier */
  type: string;
  /** Category this node belongs to */
  category: NodeCategory;
  /** Human-readable display name */
  label: string;
  /** Short description */
  description: string;
  /** Icon identifier (maps to icon set) */
  icon: string;
  /** Available input ports */
  inputs: Port[];
  /** Available output ports */
  outputs: Port[];
  /** Default configuration properties */
  defaultConfig: Record<string, unknown>;
  /** Color theme for the node (hex) */
  color: string;
}
