using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cortikal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CanvasController : ControllerBase
{
    private readonly IArchParser _parser;
    private readonly IArchitectureGenerator _generator;
    private readonly ILogger<CanvasController> _logger;

    public CanvasController(IArchParser parser, IArchitectureGenerator generator, ILogger<CanvasController> logger)
    {
        _parser = parser;
        _generator = generator;
        _logger = logger;
    }

    /// <summary>
    /// Parses an arch.md file content and returns the structured graph.
    /// </summary>
    [HttpPost("parse")]
    public ActionResult<ArchParseResult> ParseArchMd([FromBody] string markdownContent)
    {
        try
        {
            var result = _parser.Parse(markdownContent);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse arch.md content");
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Serializes a graph back into arch.md format.
    /// </summary>
    [HttpPost("serialize")]
    public ActionResult<string> SerializeArchMd([FromBody] ArchDocument document)
    {
        try
        {
            var markdown = _parser.Serialize(document);
            return Ok(markdown);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to serialize ArchDocument");
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Generates an architecture graph from a natural language prompt using AI.
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ArchParseResult>> GenerateFromPrompt(
        [FromBody] GenerateRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new { Error = "Prompt cannot be empty." });
        }

        _logger.LogInformation("Architecture generation requested for prompt: {Prompt}", request.Prompt);

        var result = await _generator.GenerateFromPromptAsync(request.Prompt, cancellationToken);

        if (!result.Success)
        {
            // Return 422 so frontend can display errors without treating it as a network failure
            return UnprocessableEntity(result);
        }

        return Ok(result);
    }
}

/// <summary>Request body for the generate endpoint.</summary>
public record GenerateRequest(string Prompt);
