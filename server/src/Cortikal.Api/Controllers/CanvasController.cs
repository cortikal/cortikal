using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cortikal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CanvasController : ControllerBase
{
    private readonly IArchParser _parser;
    private readonly ILogger<CanvasController> _logger;

    public CanvasController(IArchParser parser, ILogger<CanvasController> logger)
    {
        _parser = parser;
        _logger = logger;
    }

    /// <summary>
    /// Parses an arch.md file content and returns the structured graph.
    /// Used by frontend when user imports an arch.md file.
    /// </summary>
    [HttpPost("parse")]
    public ActionResult<ArchDocument> ParseArchMd([FromBody] string markdownContent)
    {
        try
        {
            var document = _parser.Parse(markdownContent);
            return Ok(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse arch.md content");
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Serializes a graph back into an arch.md format string.
    /// Used by frontend when saving the canvas.
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
}
