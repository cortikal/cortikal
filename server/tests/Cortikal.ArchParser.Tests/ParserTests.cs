using Cortikal.ArchParser;
using Cortikal.ArchParser.Validation;

namespace Cortikal.ArchParser.Tests;

public class ParserTests
{
    private readonly ArchMarkdownParser _parser = new();

    private static string LoadFixture(string name)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", name);
        return File.ReadAllText(path);
    }

    // ============================================================
    // Parsing — Valid Documents
    // ============================================================

    [Fact]
    public void Parse_SimpleWebApp_ReturnsSuccess()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var result = _parser.Parse(content);

        Assert.True(result.Success);
        Assert.NotNull(result.Document);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Parse_SimpleWebApp_ExtractsMetadata()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var result = _parser.Parse(content);
        var meta = result.Document!.Metadata;

        Assert.Equal("Simple Web App", meta.Name);
        Assert.Equal("cortikal-test", meta.Author);
        Assert.Equal("1.0.0", meta.Version);
        Assert.Equal("simple", meta.Complexity);
        Assert.Contains("react", meta.Tags);
        Assert.Contains("nodejs", meta.Tags);
        Assert.Contains("postgresql", meta.Tags);
    }

    [Fact]
    public void Parse_SimpleWebApp_ExtractsDescription()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var result = _parser.Parse(content);

        Assert.NotNull(result.Document!.Description);
        Assert.Contains("Simple Web Application", result.Document.Description);
    }

    [Fact]
    public void Parse_SimpleWebApp_ExtractsNodes()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var result = _parser.Parse(content);
        var nodes = result.Document!.Graph.Nodes;

        Assert.Equal(3, nodes.Count);
        Assert.Equal("frontend", nodes[0].Id);
        Assert.Equal("api", nodes[1].Id);
        Assert.Equal("database", nodes[2].Id);
    }

    [Fact]
    public void Parse_SimpleWebApp_ExtractsNodeCategories()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var result = _parser.Parse(content);
        var nodes = result.Document!.Graph.Nodes;

        Assert.Equal("frontend", nodes[0].Category);
        Assert.Equal("backend", nodes[1].Category);
        Assert.Equal("database", nodes[2].Category);
    }

    [Fact]
    public void Parse_SimpleWebApp_ExtractsPositions()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var result = _parser.Parse(content);
        var frontend = result.Document!.Graph.Nodes[0];

        Assert.Equal(100, frontend.Position.X);
        Assert.Equal(200, frontend.Position.Y);
    }

    [Fact]
    public void Parse_SimpleWebApp_ExtractsPorts()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var result = _parser.Parse(content);
        var api = result.Document!.Graph.Nodes[1];

        Assert.Single(api.Inputs);
        Assert.Single(api.Outputs);
        Assert.Equal("http-in", api.Inputs[0].Id);
        Assert.Equal("http", api.Inputs[0].DataType);
        Assert.True(api.Inputs[0].Required);
    }

    [Fact]
    public void Parse_SimpleWebApp_ExtractsEdges()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var result = _parser.Parse(content);
        var edges = result.Document!.Graph.Edges;

        Assert.Equal(2, edges.Count);
        Assert.Equal("e1", edges[0].Id);
        Assert.Equal("frontend", edges[0].SourceNodeId);
        Assert.Equal("api-out", edges[0].SourcePortId);
        Assert.Equal("api", edges[0].TargetNodeId);
        Assert.Equal("http-in", edges[0].TargetPortId);
        Assert.Equal("http", edges[0].DataType);
        Assert.Equal("dataflow", edges[0].EdgeType);
    }

    [Fact]
    public void Parse_Microservices_ReturnsSuccess()
    {
        var content = LoadFixture("microservices.arch.md");
        var result = _parser.Parse(content);

        Assert.True(result.Success);
        Assert.NotNull(result.Document);
        Assert.Equal(7, result.Document.Graph.Nodes.Count);
        Assert.Equal(6, result.Document.Graph.Edges.Count);
    }

    [Fact]
    public void Parse_Microservices_ExtractsTimestamps()
    {
        var content = LoadFixture("microservices.arch.md");
        var result = _parser.Parse(content);
        var meta = result.Document!.Metadata;

        Assert.NotNull(meta.CreatedAt);
        Assert.NotNull(meta.UpdatedAt);
    }

    [Fact]
    public void Parse_Microservices_ExtractsConfig()
    {
        var content = LoadFixture("microservices.arch.md");
        var result = _parser.Parse(content);
        var gateway = result.Document!.Graph.Nodes.First(n => n.Id == "api-gateway");

        Assert.NotEmpty(gateway.Config);
    }

    // ============================================================
    // Parsing — Invalid Documents
    // ============================================================

    [Fact]
    public void Parse_NoFrontmatter_ReturnsFail()
    {
        var content = LoadFixture("invalid-no-frontmatter.arch.md");
        var result = _parser.Parse(content);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, e => e.Contains("frontmatter"));
    }

    [Fact]
    public void Parse_EmptyContent_ReturnsFail()
    {
        var result = _parser.Parse("");

        Assert.False(result.Success);
        Assert.Contains(result.Errors, e => e.Contains("empty"));
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsFail()
    {
        var result = _parser.Parse("   \n  \n  ");

        Assert.False(result.Success);
    }

    [Fact]
    public void Parse_FrontmatterButNoArchBlock_ReturnsFail()
    {
        var content = "---\nname: Test\nauthor: test\nversion: 1.0.0\ntags: []\ncomplexity: simple\ndescription: test\n---\n\nSome description but no arch block.";
        var result = _parser.Parse(content);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, e => e.Contains("arch"));
    }
}
