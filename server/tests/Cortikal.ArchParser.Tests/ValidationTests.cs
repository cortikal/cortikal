using Cortikal.ArchParser;
using Cortikal.ArchParser.Validation;

namespace Cortikal.ArchParser.Tests;

public class ValidationTests
{
    private readonly ArchMarkdownParser _parser = new();

    private static string LoadFixture(string name)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", name);
        return File.ReadAllText(path);
    }

    // ============================================================
    // Valid Documents
    // ============================================================

    [Fact]
    public void Validate_SimpleWebApp_IsValid()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var doc = _parser.Parse(content).Document!;
        var result = ArchValidator.Validate(doc);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_Microservices_IsValid()
    {
        var content = LoadFixture("microservices.arch.md");
        var doc = _parser.Parse(content).Document!;
        var result = ArchValidator.Validate(doc);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // ============================================================
    // Invalid Documents
    // ============================================================

    [Fact]
    public void Validate_InvalidSchema_DetectsNonexistentTargetNode()
    {
        var content = LoadFixture("invalid-schema.arch.md");
        var doc = _parser.Parse(content).Document!;
        var result = ArchValidator.Validate(doc);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("node-nonexistent"));
    }

    [Fact]
    public void Validate_InvalidSchema_DetectsDuplicateEdgeIds()
    {
        var content = LoadFixture("invalid-schema.arch.md");
        var doc = _parser.Parse(content).Document!;
        var result = ArchValidator.Validate(doc);

        Assert.Contains(result.Errors, e => e.Contains("Duplicate edge ID"));
    }

    [Fact]
    public void Validate_InvalidSchema_DetectsSelfLoop()
    {
        var content = LoadFixture("invalid-schema.arch.md");
        var doc = _parser.Parse(content).Document!;
        var result = ArchValidator.Validate(doc);

        Assert.Contains(result.Errors, e => e.Contains("Self-loop"));
    }

    [Fact]
    public void Validate_TypeMismatch_DetectsIncompatibleTypes()
    {
        // Construct a document with incompatible port types (websocket → sql)
        var doc = new Core.Models.ArchDocument
        {
            Metadata = new Core.Models.ArchMetadata { Name = "Test" },
            Graph = new Core.Models.ArchGraph
            {
                Nodes =
                [
                    new Core.Models.Node
                    {
                        Id = "source",
                        Type = "test",
                        Category = "frontend",
                        Label = "Source",
                        Outputs = [new Core.Models.Port { Id = "ws-out", Direction = "output", DataType = "websocket" }]
                    },
                    new Core.Models.Node
                    {
                        Id = "target",
                        Type = "test",
                        Category = "database",
                        Label = "Target",
                        Inputs = [new Core.Models.Port { Id = "sql-in", Direction = "input", DataType = "sql" }]
                    }
                ],
                Edges =
                [
                    new Core.Models.Edge
                    {
                        Id = "e1",
                        SourceNodeId = "source",
                        SourcePortId = "ws-out",
                        TargetNodeId = "target",
                        TargetPortId = "sql-in",
                        DataType = "binary", // doesn't match either port
                        EdgeType = "dataflow"
                    }
                ]
            }
        };

        var result = ArchValidator.Validate(doc);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Type mismatch") || e.Contains("mismatch"));
    }

    // ============================================================
    // Edge Cases
    // ============================================================

    [Fact]
    public void Validate_EmptyMetadataName_HasError()
    {
        var doc = new Core.Models.ArchDocument
        {
            Metadata = new Core.Models.ArchMetadata { Name = "" },
            Graph = new Core.Models.ArchGraph
            {
                Nodes =
                [
                    new Core.Models.Node
                    {
                        Id = "test",
                        Type = "test",
                        Category = "frontend",
                        Label = "Test"
                    }
                ]
            }
        };

        var result = ArchValidator.Validate(doc);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("name"));
    }

    [Fact]
    public void Validate_InvalidCategory_HasError()
    {
        var doc = new Core.Models.ArchDocument
        {
            Metadata = new Core.Models.ArchMetadata { Name = "Test" },
            Graph = new Core.Models.ArchGraph
            {
                Nodes =
                [
                    new Core.Models.Node
                    {
                        Id = "test",
                        Type = "test",
                        Category = "invalid-category",
                        Label = "Test"
                    }
                ]
            }
        };

        var result = ArchValidator.Validate(doc);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("invalid-category"));
    }

    [Fact]
    public void Validate_OrphanNode_HasWarning()
    {
        var doc = new Core.Models.ArchDocument
        {
            Metadata = new Core.Models.ArchMetadata { Name = "Test" },
            Graph = new Core.Models.ArchGraph
            {
                Nodes =
                [
                    new Core.Models.Node
                    {
                        Id = "connected",
                        Type = "test",
                        Category = "frontend",
                        Label = "Connected",
                        Outputs = [new Core.Models.Port { Id = "out", Direction = "output", DataType = "http" }]
                    },
                    new Core.Models.Node
                    {
                        Id = "orphan",
                        Type = "test",
                        Category = "backend",
                        Label = "Orphan"
                    },
                    new Core.Models.Node
                    {
                        Id = "target",
                        Type = "test",
                        Category = "database",
                        Label = "Target",
                        Inputs = [new Core.Models.Port { Id = "in", Direction = "input", DataType = "http" }]
                    }
                ],
                Edges =
                [
                    new Core.Models.Edge
                    {
                        Id = "e1",
                        SourceNodeId = "connected",
                        SourcePortId = "out",
                        TargetNodeId = "target",
                        TargetPortId = "in",
                        DataType = "http",
                        EdgeType = "dataflow"
                    }
                ]
            }
        };

        var result = ArchValidator.Validate(doc);

        Assert.True(result.IsValid); // Orphan is a warning, not an error
        Assert.Contains(result.Warnings, w => w.Contains("orphan"));
    }
}
