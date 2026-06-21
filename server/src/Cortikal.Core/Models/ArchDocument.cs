namespace Cortikal.Core.Models;

/// <summary>
/// Represents a complete arch.md document — the core data structure
/// that maps directly to the Architecture-as-Code file format.
/// </summary>
public class ArchDocument
{
    /// <summary>YAML frontmatter metadata.</summary>
    public ArchMetadata Metadata { get; set; } = new();

    /// <summary>The architecture graph (nodes and edges).</summary>
    public ArchGraph Graph { get; set; } = new();

    /// <summary>Optional markdown description between frontmatter and graph block.</summary>
    public string? Description { get; set; }
}

/// <summary>
/// YAML frontmatter metadata for an arch.md file.
/// </summary>
public class ArchMetadata
{
    public string Name { get; set; } = "Untitled";
    public string Author { get; set; } = "Unknown";
    public string Version { get; set; } = "0.1.0";
    public List<string> Tags { get; set; } = [];
    public string Complexity { get; set; } = "simple";
    public string Description { get; set; } = "";
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// The architecture graph containing nodes and edges.
/// </summary>
public class ArchGraph
{
    public List<Node> Nodes { get; set; } = [];
    public List<Edge> Edges { get; set; } = [];
}
