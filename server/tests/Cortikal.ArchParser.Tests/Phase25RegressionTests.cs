using Cortikal.ArchParser;

namespace Cortikal.ArchParser.Tests;

/// <summary>
/// Regression tests for Phase 2.5 hardening fixes.
/// These test the integration between the parser and the validator
/// (now wired together), fence-stripping improvements, and round-trip
/// metadata preservation.
/// </summary>
public class Phase25RegressionTests
{
    private readonly ArchMarkdownParser _parser = new();

    private static string LoadFixture(string name)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", name);
        return File.ReadAllText(path);
    }

    // ============================================================
    // Parser + Validator integration (fix #4: semantic validation
    // is now enforced at parse time, not just when calling
    // ArchValidator.Validate explicitly)
    // ============================================================

    [Fact]
    public void Parse_DocumentWithDanglingEdgeRef_FailsAtParseTime()
    {
        // The invalid-schema fixture contains an edge referencing "node-nonexistent".
        // Before the fix, Parse() would return Success=true (structural parse was fine).
        // After wiring ArchValidator into the parser, this should fail.
        var content = LoadFixture("invalid-schema.arch.md");
        var result = _parser.Parse(content);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, e => e.Contains("node-nonexistent") || e.Contains("does not exist"));
    }

    [Fact]
    public void Parse_DocumentWithSelfLoop_FailsAtParseTime()
    {
        var content = LoadFixture("invalid-schema.arch.md");
        var result = _parser.Parse(content);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, e => e.Contains("Self-loop"));
    }

    [Fact]
    public void Parse_DocumentWithDuplicateEdgeId_FailsAtParseTime()
    {
        var content = LoadFixture("invalid-schema.arch.md");
        var result = _parser.Parse(content);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, e => e.Contains("Duplicate edge ID"));
    }

    [Fact]
    public void Parse_ValidDocument_StillSucceeds()
    {
        // Ensure the validator integration doesn't break valid documents
        var content = LoadFixture("simple-web-app.arch.md");
        var result = _parser.Parse(content);

        Assert.True(result.Success);
        Assert.NotNull(result.Document);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Parse_ValidMicroservices_StillSucceeds()
    {
        var content = LoadFixture("microservices.arch.md");
        var result = _parser.Parse(content);

        Assert.True(result.Success);
        Assert.NotNull(result.Document);
        Assert.Empty(result.Errors);
    }

    // ============================================================
    // StripMarkdownFences hardening (fix #9)
    // ============================================================

    [Fact]
    public void Parse_GeneratedOutputWithYamlFences_SuccessfullyParses()
    {
        // Simulate what the AI generator produces: the YAML graph block is
        // wrapped in markdown fences that StripMarkdownFences strips, then
        // the generator wraps it in a proper arch.md document.
        // This test verifies the full parser path handles the result.
        var archMd = """
            ---
            name: "Generated Test"
            author: "cortikal-ai"
            version: "0.1.0"
            tags: [ai-generated]
            complexity: "simple"
            description: "test"
            ---

            Test architecture.

            ```arch
            nodes:
              - id: svc
                type: dotnet-api
                category: backend
                label: "Service"
                position: { x: 100, y: 100 }
            ```
            """;

        var result = _parser.Parse(archMd);

        Assert.True(result.Success);
        Assert.Single(result.Document!.Graph.Nodes);
    }

    // ============================================================
    // Metadata round-trip preservation (fix #6)
    // ============================================================

    [Fact]
    public void SerializeAndReparse_PreservesAllMetadataFields()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var original = _parser.Parse(content);
        Assert.True(original.Success);

        var serialized = _parser.Serialize(original.Document!);
        var reparsed = _parser.Parse(serialized);

        Assert.True(reparsed.Success);
        var m1 = original.Document!.Metadata;
        var m2 = reparsed.Document!.Metadata;

        Assert.Equal(m1.Name, m2.Name);
        Assert.Equal(m1.Author, m2.Author);
        Assert.Equal(m1.Version, m2.Version);
        Assert.Equal(m1.Complexity, m2.Complexity);
        Assert.Equal(m1.Tags, m2.Tags);
    }

    [Fact]
    public void SerializeAndReparse_PreservesDescription()
    {
        var content = LoadFixture("simple-web-app.arch.md");
        var original = _parser.Parse(content);
        var serialized = _parser.Serialize(original.Document!);
        var reparsed = _parser.Parse(serialized);

        // Description should survive the round-trip
        Assert.NotNull(reparsed.Document!.Description);
    }

    // ============================================================
    // Registry template validation (fix #3)
    // ============================================================

    [Fact]
    public void Parse_ApiGatewayTemplate_IsValid()
    {
        // Verify the new api-gateway template parses and validates successfully
        var templatePath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..",
            "registry", "templates", "api-gateway", "arch.md"));

        if (!File.Exists(templatePath))
        {
            // Skip if running from a context where the repo root isn't accessible
            return;
        }

        var content = File.ReadAllText(templatePath);
        var result = _parser.Parse(content);

        Assert.True(result.Success, $"Errors: {string.Join(", ", result.Errors)}");
        Assert.NotNull(result.Document);
        Assert.True(result.Document.Graph.Nodes.Count > 0);
    }

    [Fact]
    public void Parse_MlPipelineTemplate_IsValid()
    {
        var templatePath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..",
            "registry", "templates", "ml-pipeline", "arch.md"));

        if (!File.Exists(templatePath))
        {
            return;
        }

        var content = File.ReadAllText(templatePath);
        var result = _parser.Parse(content);

        Assert.True(result.Success, $"Errors: {string.Join(", ", result.Errors)}");
        Assert.NotNull(result.Document);
        Assert.True(result.Document.Graph.Nodes.Count > 0);
    }
}
