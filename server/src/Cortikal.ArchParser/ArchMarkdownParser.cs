using System.Text.RegularExpressions;
using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cortikal.ArchParser;

/// <summary>
/// Parses arch.md files into <see cref="ArchDocument"/> models.
/// Handles YAML frontmatter extraction and ```arch graph block parsing.
/// </summary>
public partial class ArchMarkdownParser : IArchParser
{
    private static readonly Regex FrontmatterRegex = CreateFrontmatterRegex();
    private static readonly Regex ArchBlockRegex = CreateArchBlockRegex();

    private readonly IDeserializer _yamlDeserializer;
    private readonly ISerializer _yamlSerializer;

    public ArchMarkdownParser()
    {
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        _yamlSerializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();
    }

    /// <inheritdoc />
    public ArchParseResult Parse(string content)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        if (string.IsNullOrWhiteSpace(content))
        {
            errors.Add("Content is empty.");
            return new ArchParseResult { Success = false, Errors = errors };
        }

        // 1. Extract YAML frontmatter
        var frontmatterMatch = FrontmatterRegex.Match(content);
        if (!frontmatterMatch.Success)
        {
            errors.Add("Missing YAML frontmatter. Expected --- delimited block at the start of the file.");
            return new ArchParseResult { Success = false, Errors = errors };
        }

        ArchMetadata metadata;
        try
        {
            metadata = _yamlDeserializer.Deserialize<ArchMetadata>(frontmatterMatch.Groups[1].Value)
                        ?? new ArchMetadata();
        }
        catch (Exception ex)
        {
            errors.Add($"Failed to parse YAML frontmatter: {ex.Message}");
            return new ArchParseResult { Success = false, Errors = errors };
        }

        // Validate required metadata fields
        if (string.IsNullOrWhiteSpace(metadata.Name) || metadata.Name == "Untitled")
        {
            warnings.Add("Architecture name is missing or set to 'Untitled'.");
        }

        // 2. Extract description (markdown between frontmatter and arch block)
        var afterFrontmatter = content[frontmatterMatch.Length..];
        var archBlockMatch = ArchBlockRegex.Match(afterFrontmatter);

        string? description = null;
        if (archBlockMatch.Success)
        {
            var beforeArchBlock = afterFrontmatter[..archBlockMatch.Index].Trim();
            if (!string.IsNullOrWhiteSpace(beforeArchBlock))
            {
                description = beforeArchBlock;
            }
        }

        // 3. Extract and parse ```arch block
        if (!archBlockMatch.Success)
        {
            errors.Add("Missing ```arch code block. The file must contain a fenced code block with language 'arch'.");
            return new ArchParseResult { Success = false, Errors = errors };
        }

        ArchGraphYaml graphYaml;
        try
        {
            graphYaml = _yamlDeserializer.Deserialize<ArchGraphYaml>(archBlockMatch.Groups[1].Value)
                        ?? new ArchGraphYaml();
        }
        catch (Exception ex)
        {
            errors.Add($"Failed to parse arch block YAML: {ex.Message}");
            return new ArchParseResult { Success = false, Errors = errors };
        }

        // 4. Convert to domain model
        var graph = ConvertGraph(graphYaml);

        var document = new ArchDocument
        {
            Metadata = metadata,
            Graph = graph,
            Description = description
        };

        return new ArchParseResult
        {
            Success = errors.Count == 0,
            Document = document,
            Errors = errors,
            Warnings = warnings
        };
    }

    /// <inheritdoc />
    public string Serialize(ArchDocument document)
    {
        return ArchSerializer.Serialize(document, _yamlSerializer);
    }

    private static ArchGraph ConvertGraph(ArchGraphYaml yaml)
    {
        var graph = new ArchGraph();

        if (yaml.Nodes != null)
        {
            foreach (var nodeYaml in yaml.Nodes)
            {
                var node = new Node
                {
                    Id = nodeYaml.Id ?? string.Empty,
                    Type = nodeYaml.Type ?? string.Empty,
                    Category = nodeYaml.Category ?? string.Empty,
                    Label = nodeYaml.Label ?? string.Empty,
                    Position = new NodePosition
                    {
                        X = nodeYaml.Position?.X ?? 0,
                        Y = nodeYaml.Position?.Y ?? 0
                    },
                    Config = nodeYaml.Config ?? [],
                    Inputs = ConvertPorts(nodeYaml.Inputs),
                    Outputs = ConvertPorts(nodeYaml.Outputs)
                };
                graph.Nodes.Add(node);
            }
        }

        if (yaml.Edges != null)
        {
            foreach (var edgeYaml in yaml.Edges)
            {
                var edge = new Edge
                {
                    Id = edgeYaml.Id ?? string.Empty,
                    SourceNodeId = edgeYaml.SourceNodeId ?? string.Empty,
                    SourcePortId = edgeYaml.SourcePortId ?? string.Empty,
                    TargetNodeId = edgeYaml.TargetNodeId ?? string.Empty,
                    TargetPortId = edgeYaml.TargetPortId ?? string.Empty,
                    DataType = edgeYaml.DataType ?? "json",
                    EdgeType = edgeYaml.EdgeType ?? "dataflow",
                    Label = edgeYaml.Label
                };
                graph.Edges.Add(edge);
            }
        }

        return graph;
    }

    private static List<Port> ConvertPorts(List<PortYaml>? portYamls)
    {
        if (portYamls == null) return [];

        return portYamls.Select(p => new Port
        {
            Id = p.Id ?? string.Empty,
            Label = p.Label ?? string.Empty,
            Direction = p.Direction ?? "input",
            DataType = p.DataType ?? "json",
            Required = p.Required,
            Description = p.Description
        }).ToList();
    }

    [GeneratedRegex(@"^---\s*\r?\n([\s\S]*?)\r?\n---", RegexOptions.None)]
    private static partial Regex CreateFrontmatterRegex();

    [GeneratedRegex(@"```arch\s*\r?\n([\s\S]*?)\r?\n```", RegexOptions.None)]
    private static partial Regex CreateArchBlockRegex();
}

// ============================================================
// Internal YAML deserialization models
// ============================================================

internal class ArchGraphYaml
{
    public List<NodeYaml>? Nodes { get; set; }
    public List<EdgeYaml>? Edges { get; set; }
}

internal class NodeYaml
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? Label { get; set; }
    public PositionYaml? Position { get; set; }
    public Dictionary<string, object?>? Config { get; set; }
    public List<PortYaml>? Inputs { get; set; }
    public List<PortYaml>? Outputs { get; set; }
}

internal class PositionYaml
{
    public double X { get; set; }
    public double Y { get; set; }
}

internal class PortYaml
{
    public string? Id { get; set; }
    public string? Label { get; set; }
    public string? Direction { get; set; }
    public string? DataType { get; set; }
    public bool Required { get; set; }
    public string? Description { get; set; }
}

internal class EdgeYaml
{
    public string? Id { get; set; }
    public string? SourceNodeId { get; set; }
    public string? SourcePortId { get; set; }
    public string? TargetNodeId { get; set; }
    public string? TargetPortId { get; set; }
    public string? DataType { get; set; }
    public string? EdgeType { get; set; }
    public string? Label { get; set; }
}
