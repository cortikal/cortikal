namespace Cortikal.Core.Models;

/// <summary>
/// Represents an I/O port on a node with strict type contracts.
/// </summary>
public class Port
{
    /// <summary>Unique identifier within the node.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Human-readable label.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Direction: "input" or "output".</summary>
    public string Direction { get; set; } = "input";

    /// <summary>Data type for contract enforcement.</summary>
    public string DataType { get; set; } = "json";

    /// <summary>Whether this port is required.</summary>
    public bool Required { get; set; } = false;

    /// <summary>Optional description.</summary>
    public string? Description { get; set; }
}
