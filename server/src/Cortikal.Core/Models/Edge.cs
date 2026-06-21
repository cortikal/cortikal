namespace Cortikal.Core.Models;

/// <summary>
/// Represents a connection between two nodes in the architecture graph.
/// </summary>
public class Edge
{
    /// <summary>Unique edge identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Source node ID.</summary>
    public string SourceNodeId { get; set; } = string.Empty;

    /// <summary>Source port ID.</summary>
    public string SourcePortId { get; set; } = string.Empty;

    /// <summary>Target node ID.</summary>
    public string TargetNodeId { get; set; } = string.Empty;

    /// <summary>Target port ID.</summary>
    public string TargetPortId { get; set; } = string.Empty;

    /// <summary>Data type flowing through this edge.</summary>
    public string DataType { get; set; } = "json";

    /// <summary>Edge type: "dataflow" or "dependency".</summary>
    public string EdgeType { get; set; } = "dataflow";

    /// <summary>Optional label.</summary>
    public string? Label { get; set; }
}
