using System.Net.Http.Json;
using System.Text.Json;
using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cortikal.Infrastructure.Generation;

/// <summary>
/// Generates architecture graphs from natural language prompts using OpenAI.
/// </summary>
public class ArchitectureGeneratorService : IArchitectureGenerator
{
    private readonly HttpClient _httpClient;
    private readonly IArchParser _parser;
    private readonly ILogger<ArchitectureGeneratorService> _logger;
    private readonly string _apiKey;
    private readonly string _model;

    private const string SystemPrompt = """
        You are an expert software architect for the Cortikal visual architecture platform.
        Generate a software architecture graph in YAML format based on the user's system description.

        Output ONLY the raw YAML content — no markdown fences, no frontmatter, no explanations, no commentary.

        Required YAML structure:
        nodes:
          - id: <kebab-case-unique-id>
            type: <technology e.g. react-app, nextjs-app, dotnet-api, node-api, postgresql, mysql, mongodb, redis, nginx, docker, kubernetes, rabbitmq, kafka, s3, openai-api>
            category: <frontend|backend|database|infrastructure|ai|integration>
            label: "<Human readable name>"
            position: { x: <number>, y: <number> }
            inputs:
              - id: <port-id>
                label: "<port label>"
                direction: input
                dataType: <http|json|sql|grpc|event|stream|ws>
                required: <true|false>
            outputs:
              - id: <port-id>
                label: "<port label>"
                direction: output
                dataType: <http|json|sql|grpc|event|stream|ws>
                required: <true|false>
        edges:
          - id: e1
            sourceNodeId: <source node id>
            sourcePortId: <source output port id>
            targetNodeId: <target node id>
            targetPortId: <target input port id>
            dataType: <http|json|sql|grpc|event|stream|ws>
            edgeType: <dataflow|dependency>
            label: "<connection label>"

        Layout rules (positions in pixels):
        - Arrange nodes left-to-right by layer: frontend (x≈100), api-gateway (x≈420), services (x≈740), databases (x≈1060)
        - Space nodes vertically: start at y=100, add 200 for each additional node in the same column
        - Aim for 5-9 nodes for a typical system
        - Every output port wired in an edge must exist on the source node; every input port wired must exist on the target node
        - Node IDs must be unique kebab-case strings (no spaces)

        Output ONLY valid YAML, nothing else.
        """;

    public ArchitectureGeneratorService(
        HttpClient httpClient,
        IArchParser parser,
        IConfiguration configuration,
        ILogger<ArchitectureGeneratorService> logger)
    {
        _httpClient = httpClient;
        _parser = parser;
        _logger = logger;
        _apiKey = configuration["Cortikal:OpenAi:ApiKey"] ?? string.Empty;
        _model = configuration["Cortikal:OpenAi:Model"] ?? "gpt-4o";
    }

    public async Task<ArchParseResult> GenerateFromPromptAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            return new ArchParseResult
            {
                Success = false,
                Errors = ["OpenAI API key is not configured. Add your key to Cortikal:OpenAi:ApiKey in appsettings or as an environment variable."]
            };
        }

        _logger.LogInformation("Generating architecture for prompt: {Prompt}", prompt);

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = prompt }
                },
                temperature = 0.3,
                max_tokens = 3000
            };

            var response = await _httpClient.PostAsJsonAsync(
                "https://api.openai.com/v1/chat/completions",
                requestBody,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OpenAI API error {StatusCode}: {Body}", response.StatusCode, errorBody);
                return new ArchParseResult
                {
                    Success = false,
                    Errors = [$"OpenAI API returned {(int)response.StatusCode}: {response.ReasonPhrase}"]
                };
            }

            using var jsonDoc = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync(cancellationToken),
                cancellationToken: cancellationToken);

            var yamlContent = jsonDoc
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            // Strip any accidental markdown fences the model might output
            yamlContent = StripMarkdownFences(yamlContent);

            // Escape double quotes in prompt for embedding in YAML string
            var safePrompt = prompt.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", " ");

            // Wrap the YAML graph block into a complete arch.md document
            var archMd = $"""
                ---
                name: "Generated Architecture"
                author: "cortikal-ai"
                version: "0.1.0"
                tags: [ai-generated]
                complexity: "moderate"
                description: "{safePrompt}"
                createdAt: "{DateTime.UtcNow:O}"
                ---

                AI-generated architecture for: {prompt}

                ```arch
                {yamlContent.Trim()}
                ```
                """;

            var result = _parser.Parse(archMd);

            if (!result.Success)
            {
                _logger.LogWarning("Generated arch.md failed to parse. Errors: {Errors}", string.Join(", ", result.Errors));
                _logger.LogDebug("Generated arch.md content:\n{Content}", archMd);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            return new ArchParseResult { Success = false, Errors = ["Request was cancelled."] };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during architecture generation");
            return new ArchParseResult { Success = false, Errors = [$"Generation failed: {ex.Message}"] };
        }
    }

    private static string StripMarkdownFences(string content)
    {
        var lines = content.Split('\n');
        var filtered = new List<string>();
        bool inFence = false;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("```"))
            {
                inFence = !inFence;
                continue;
            }
            filtered.Add(line);
        }

        return string.Join('\n', filtered);
    }
}
