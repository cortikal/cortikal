namespace Cortikal.Core.Models;

/// <summary>
/// Represents a single node in the architecture graph.
/// </summary>
public class Node
{
    /// <summary>Unique node identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Node type identifier (e.g., "react-app", "dotnet-api").</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Category: frontend, backend, database, infrastructure, ai.</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Human-readable display label.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Canvas position.</summary>
    public NodePosition Position { get; set; } = new();

    /// <summary>Node-specific configuration properties.</summary>
    public Dictionary<string, object?> Config { get; set; } = [];

    /// <summary>Input ports.</summary>
    public List<Port> Inputs { get; set; } = [];

    /// <summary>Output ports.</summary>
    public List<Port> Outputs { get; set; } = [];
}

/// <summary>
/// Canvas position for a node.
/// </summary>
public class NodePosition
{
    public double X { get; set; }
    public double Y { get; set; }
}
