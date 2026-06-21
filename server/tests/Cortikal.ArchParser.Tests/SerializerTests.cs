using Cortikal.ArchParser;

namespace Cortikal.ArchParser.Tests;

public class SerializerTests
{
    private readonly ArchMarkdownParser _parser = new();

    private static string LoadFixture(string name)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", name);
        return File.ReadAllText(path);
    }

    [Fact]
    public void Serialize_RoundTrip_PreservesMetadata()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var parseResult = _parser.Parse(content);
        Assert.True(parseResult.Success);

        var serialized = _parser.Serialize(parseResult.Document!);
        var reparsed = _parser.Parse(serialized);

        Assert.True(reparsed.Success);
        Assert.Equal(parseResult.Document!.Metadata.Name, reparsed.Document!.Metadata.Name);
        Assert.Equal(parseResult.Document.Metadata.Author, reparsed.Document.Metadata.Author);
        Assert.Equal(parseResult.Document.Metadata.Version, reparsed.Document.Metadata.Version);
        Assert.Equal(parseResult.Document.Metadata.Complexity, reparsed.Document.Metadata.Complexity);
    }

    [Fact]
    public void Serialize_RoundTrip_PreservesNodeCount()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var parseResult = _parser.Parse(content);
        var serialized = _parser.Serialize(parseResult.Document!);
        var reparsed = _parser.Parse(serialized);

        Assert.Equal(
            parseResult.Document!.Graph.Nodes.Count,
            reparsed.Document!.Graph.Nodes.Count
        );
    }

    [Fact]
    public void Serialize_RoundTrip_PreservesEdgeCount()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var parseResult = _parser.Parse(content);
        var serialized = _parser.Serialize(parseResult.Document!);
        var reparsed = _parser.Parse(serialized);

        Assert.Equal(
            parseResult.Document!.Graph.Edges.Count,
            reparsed.Document!.Graph.Edges.Count
        );
    }

    [Fact]
    public void Serialize_RoundTrip_PreservesNodeIds()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var parseResult = _parser.Parse(content);
        var serialized = _parser.Serialize(parseResult.Document!);
        var reparsed = _parser.Parse(serialized);

        var originalIds = parseResult.Document!.Graph.Nodes.Select(n => n.Id).OrderBy(x => x);
        var reparsedIds = reparsed.Document!.Graph.Nodes.Select(n => n.Id).OrderBy(x => x);
        Assert.Equal(originalIds, reparsedIds);
    }

    [Fact]
    public void Serialize_RoundTrip_PreservesPortData()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var parseResult = _parser.Parse(content);
        var serialized = _parser.Serialize(parseResult.Document!);
        var reparsed = _parser.Parse(serialized);

        var originalApi = parseResult.Document!.Graph.Nodes.First(n => n.Id == "api");
        var reparsedApi = reparsed.Document!.Graph.Nodes.First(n => n.Id == "api");

        Assert.Equal(originalApi.Inputs.Count, reparsedApi.Inputs.Count);
        Assert.Equal(originalApi.Outputs.Count, reparsedApi.Outputs.Count);
        Assert.Equal(originalApi.Inputs[0].DataType, reparsedApi.Inputs[0].DataType);
    }

    [Fact]
    public void Serialize_RoundTrip_Microservices()
    {
        var content = LoadFixture("microservices.arch.md");
        var parseResult = _parser.Parse(content);
        var serialized = _parser.Serialize(parseResult.Document!);
        var reparsed = _parser.Parse(serialized);

        Assert.True(reparsed.Success);
        Assert.Equal(7, reparsed.Document!.Graph.Nodes.Count);
        Assert.Equal(6, reparsed.Document.Graph.Edges.Count);
    }

    [Fact]
    public void Serialize_OutputContainsFrontmatter()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var parseResult = _parser.Parse(content);
        var serialized = _parser.Serialize(parseResult.Document!);

        Assert.StartsWith("---", serialized);
        Assert.Contains("name: Simple Web App", serialized);
    }

    [Fact]
    public void Serialize_OutputContainsArchBlock()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var parseResult = _parser.Parse(content);
        var serialized = _parser.Serialize(parseResult.Document!);

        Assert.Contains("```arch", serialized);
        Assert.Contains("```", serialized);
        Assert.Contains("nodes:", serialized);
        Assert.Contains("edges:", serialized);
    }
}
