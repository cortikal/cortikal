using Cortikal.Core.Models;

namespace Cortikal.ArchParser.Validation;

/// <summary>
/// Validates structural integrity of an <see cref="ArchDocument"/>.
/// Checks node/edge references, unique IDs, and I/O type contract compatibility.
/// </summary>
public static class ArchValidator
{
    /// <summary>
    /// Validate an ArchDocument for structural correctness.
    /// Returns a list of errors and warnings.
    /// </summary>
    public static ArchValidationResult Validate(ArchDocument document)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        ValidateMetadata(document.Metadata, errors, warnings);
        ValidateNodes(document.Graph.Nodes, errors, warnings);
        ValidateEdges(document.Graph, errors, warnings);
        ValidateIoContracts(document.Graph, errors, warnings);

        return new ArchValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors,
            Warnings = warnings
        };
    }

    private static void ValidateMetadata(ArchMetadata metadata, List<string> errors, List<string> warnings)
    {
        if (string.IsNullOrWhiteSpace(metadata.Name))
            errors.Add("Metadata: 'name' is required.");

        if (string.IsNullOrWhiteSpace(metadata.Author))
            warnings.Add("Metadata: 'author' is not set.");

        var validComplexities = new[] { "simple", "moderate", "complex", "enterprise" };
        if (!validComplexities.Contains(metadata.Complexity))
            errors.Add($"Metadata: Invalid complexity '{metadata.Complexity}'. Must be one of: {string.Join(", ", validComplexities)}.");
    }

    private static void ValidateNodes(List<Node> nodes, List<string> errors, List<string> warnings)
    {
        var nodeIds = new HashSet<string>();

        foreach (var node in nodes)
        {
            // Check required fields
            if (string.IsNullOrWhiteSpace(node.Id))
            {
                errors.Add("Node is missing an 'id' field.");
                continue;
            }

            // Check unique IDs
            if (!nodeIds.Add(node.Id))
                errors.Add($"Node '{node.Id}': Duplicate node ID.");

            if (string.IsNullOrWhiteSpace(node.Type))
                errors.Add($"Node '{node.Id}': Missing 'type' field.");

            if (string.IsNullOrWhiteSpace(node.Label))
                warnings.Add($"Node '{node.Id}': Missing 'label' field.");

            // Validate category
            var validCategories = new[] { "frontend", "backend", "database", "infrastructure", "ai", "integration", "custom" };
            if (!validCategories.Contains(node.Category))
                errors.Add($"Node '{node.Id}': Invalid category '{node.Category}'. Must be one of: {string.Join(", ", validCategories)}.");

            // Validate port IDs are unique within the node
            ValidatePortIds(node, errors, warnings);
        }
    }

    private static void ValidatePortIds(Node node, List<string> errors, List<string> warnings)
    {
        var portIds = new HashSet<string>();
        var allPorts = node.Inputs.Concat(node.Outputs);

        foreach (var port in allPorts)
        {
            if (string.IsNullOrWhiteSpace(port.Id))
            {
                errors.Add($"Node '{node.Id}': Port is missing an 'id' field.");
                continue;
            }

            if (!portIds.Add(port.Id))
                errors.Add($"Node '{node.Id}': Duplicate port ID '{port.Id}'.");

            // Validate direction matches collection
            if (node.Inputs.Contains(port) && port.Direction != "input")
                warnings.Add($"Node '{node.Id}', Port '{port.Id}': Listed in inputs but direction is '{port.Direction}'.");

            if (node.Outputs.Contains(port) && port.Direction != "output")
                warnings.Add($"Node '{node.Id}', Port '{port.Id}': Listed in outputs but direction is '{port.Direction}'.");
        }
    }

    private static void ValidateEdges(ArchGraph graph, List<string> errors, List<string> warnings)
    {
        var nodeMap = graph.Nodes.ToDictionary(n => n.Id, n => n);
        var edgeIds = new HashSet<string>();

        foreach (var edge in graph.Edges)
        {
            // Check required fields
            if (string.IsNullOrWhiteSpace(edge.Id))
            {
                errors.Add("Edge is missing an 'id' field.");
                continue;
            }

            if (!edgeIds.Add(edge.Id))
                errors.Add($"Edge '{edge.Id}': Duplicate edge ID.");

            // Check node references
            if (!nodeMap.TryGetValue(edge.SourceNodeId, out var sourceNode))
            {
                errors.Add($"Edge '{edge.Id}': Source node '{edge.SourceNodeId}' does not exist.");
                continue;
            }

            if (!nodeMap.TryGetValue(edge.TargetNodeId, out var targetNode))
            {
                errors.Add($"Edge '{edge.Id}': Target node '{edge.TargetNodeId}' does not exist.");
                continue;
            }

            // Check self-loops
            if (edge.SourceNodeId == edge.TargetNodeId)
                errors.Add($"Edge '{edge.Id}': Self-loops are not allowed (node '{edge.SourceNodeId}').");

            // Check port references
            var sourcePort = sourceNode.Outputs.FirstOrDefault(p => p.Id == edge.SourcePortId);
            if (sourcePort == null)
                errors.Add($"Edge '{edge.Id}': Source port '{edge.SourcePortId}' does not exist on node '{edge.SourceNodeId}'.");

            var targetPort = targetNode.Inputs.FirstOrDefault(p => p.Id == edge.TargetPortId);
            if (targetPort == null)
                errors.Add($"Edge '{edge.Id}': Target port '{edge.TargetPortId}' does not exist on node '{edge.TargetNodeId}'.");

            // Validate edge type
            var validEdgeTypes = new[] { "dataflow", "dependency" };
            if (!validEdgeTypes.Contains(edge.EdgeType))
                errors.Add($"Edge '{edge.Id}': Invalid edge type '{edge.EdgeType}'. Must be one of: {string.Join(", ", validEdgeTypes)}.");
        }

        // Warn about orphan nodes
        if (graph.Nodes.Count > 1)
        {
            foreach (var node in graph.Nodes)
            {
                var hasConnection = graph.Edges.Any(e =>
                    e.SourceNodeId == node.Id || e.TargetNodeId == node.Id);

                if (!hasConnection)
                    warnings.Add($"Node '{node.Id}': Has no connections (orphan node).");
            }
        }
    }

    private static void ValidateIoContracts(ArchGraph graph, List<string> errors, List<string> warnings)
    {
        var nodeMap = graph.Nodes.ToDictionary(n => n.Id, n => n);

        foreach (var edge in graph.Edges)
        {
            if (!nodeMap.TryGetValue(edge.SourceNodeId, out var sourceNode) ||
                !nodeMap.TryGetValue(edge.TargetNodeId, out var targetNode))
                continue; // Already reported as invalid reference

            var sourcePort = sourceNode.Outputs.FirstOrDefault(p => p.Id == edge.SourcePortId);
            var targetPort = targetNode.Inputs.FirstOrDefault(p => p.Id == edge.TargetPortId);

            if (sourcePort == null || targetPort == null)
                continue; // Already reported as invalid reference

            // Check type compatibility
            if (!AreTypesCompatible(sourcePort.DataType, targetPort.DataType, edge.DataType))
            {
                errors.Add(
                    $"Edge '{edge.Id}': Type mismatch — source port '{sourcePort.Id}' " +
                    $"({sourcePort.DataType}) → target port '{targetPort.Id}' " +
                    $"({targetPort.DataType}) via edge type '{edge.DataType}'.");
            }
        }

        // Check required ports have connections
        foreach (var node in graph.Nodes)
        {
            foreach (var port in node.Inputs.Where(p => p.Required))
            {
                var hasConnection = graph.Edges.Any(e =>
                    e.TargetNodeId == node.Id && e.TargetPortId == port.Id);

                if (!hasConnection)
                    warnings.Add($"Node '{node.Id}', Port '{port.Id}': Required input port has no incoming connection.");
            }

            foreach (var port in node.Outputs.Where(p => p.Required))
            {
                var hasConnection = graph.Edges.Any(e =>
                    e.SourceNodeId == node.Id && e.SourcePortId == port.Id);

                if (!hasConnection)
                    warnings.Add($"Node '{node.Id}', Port '{port.Id}': Required output port has no outgoing connection.");
            }
        }
    }

    /// <summary>
    /// Check if data types are compatible for an edge connection.
    /// </summary>
    private static bool AreTypesCompatible(string sourceType, string targetType, string edgeType)
    {
        // Exact match is always compatible
        if (sourceType == targetType && sourceType == edgeType)
            return true;

        // JSON is compatible with object, array, and string
        var jsonCompatible = new HashSet<string> { "json", "object", "array", "string" };
        if (jsonCompatible.Contains(sourceType) && jsonCompatible.Contains(targetType))
            return true;

        // Allow edge type to match either source or target
        if (edgeType == sourceType || edgeType == targetType)
            return true;

        return false;
    }
}

/// <summary>
/// Result of validating an ArchDocument.
/// </summary>
public class ArchValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}
