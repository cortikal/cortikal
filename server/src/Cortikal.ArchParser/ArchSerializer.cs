using System.Text;
using Cortikal.Core.Models;
using YamlDotNet.Serialization;

namespace Cortikal.ArchParser;

/// <summary>
/// Serializes <see cref="ArchDocument"/> models back to arch.md text format.
/// Produces round-trippable output that matches the arch.md specification.
/// </summary>
public static class ArchSerializer
{
    /// <summary>
    /// Serialize an ArchDocument to arch.md format.
    /// </summary>
    public static string Serialize(ArchDocument document, ISerializer? yamlSerializer = null)
    {
        yamlSerializer ??= new SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();

        var sb = new StringBuilder();

        // 1. YAML frontmatter
        sb.AppendLine("---");
        sb.Append(SerializeFrontmatter(document.Metadata, yamlSerializer));
        sb.AppendLine("---");

        // 2. Optional description
        if (!string.IsNullOrWhiteSpace(document.Description))
        {
            sb.AppendLine();
            sb.AppendLine(document.Description);
        }

        // 3. Architecture graph block
        sb.AppendLine();
        sb.AppendLine("```arch");
        sb.Append(SerializeGraph(document.Graph, yamlSerializer));
        sb.AppendLine("```");

        return sb.ToString();
    }

    private static string SerializeFrontmatter(ArchMetadata metadata, ISerializer serializer)
    {
        var frontmatterObj = new Dictionary<string, object?>
        {
            ["name"] = metadata.Name,
            ["author"] = metadata.Author,
            ["version"] = metadata.Version,
            ["tags"] = metadata.Tags,
            ["complexity"] = metadata.Complexity,
            ["description"] = metadata.Description
        };

        if (metadata.CreatedAt.HasValue)
            frontmatterObj["createdAt"] = metadata.CreatedAt.Value.ToString("O");

        if (metadata.UpdatedAt.HasValue)
            frontmatterObj["updatedAt"] = metadata.UpdatedAt.Value.ToString("O");

        return serializer.Serialize(frontmatterObj);
    }

    private static string SerializeGraph(ArchGraph graph, ISerializer serializer)
    {
        var graphObj = new Dictionary<string, object>();

        // Serialize nodes
        var nodes = graph.Nodes.Select(n =>
        {
            var nodeDict = new Dictionary<string, object?>
            {
                ["id"] = n.Id,
                ["type"] = n.Type,
                ["category"] = n.Category,
                ["label"] = n.Label,
                ["position"] = new { x = n.Position.X, y = n.Position.Y }
            };

            if (n.Config.Count > 0)
                nodeDict["config"] = n.Config;

            if (n.Inputs.Count > 0)
                nodeDict["inputs"] = SerializePorts(n.Inputs);

            if (n.Outputs.Count > 0)
                nodeDict["outputs"] = SerializePorts(n.Outputs);

            return nodeDict;
        }).ToList();

        graphObj["nodes"] = nodes;

        // Serialize edges
        if (graph.Edges.Count > 0)
        {
            var edges = graph.Edges.Select(e =>
            {
                var edgeDict = new Dictionary<string, object?>
                {
                    ["id"] = e.Id,
                    ["sourceNodeId"] = e.SourceNodeId,
                    ["sourcePortId"] = e.SourcePortId,
                    ["targetNodeId"] = e.TargetNodeId,
                    ["targetPortId"] = e.TargetPortId,
                    ["dataType"] = e.DataType,
                    ["edgeType"] = e.EdgeType
                };

                if (!string.IsNullOrEmpty(e.Label))
                    edgeDict["label"] = e.Label;

                return edgeDict;
            }).ToList();

            graphObj["edges"] = edges;
        }

        return serializer.Serialize(graphObj);
    }

    private static List<Dictionary<string, object?>> SerializePorts(List<Port> ports)
    {
        return ports.Select(p =>
        {
            var portDict = new Dictionary<string, object?>
            {
                ["id"] = p.Id,
                ["label"] = p.Label,
                ["direction"] = p.Direction,
                ["dataType"] = p.DataType,
                ["required"] = p.Required
            };

            if (!string.IsNullOrEmpty(p.Description))
                portDict["description"] = p.Description;

            return portDict;
        }).ToList();
    }
}
