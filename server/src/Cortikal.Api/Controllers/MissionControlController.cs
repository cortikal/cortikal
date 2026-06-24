using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cortikal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MissionControlController : ControllerBase
{
    private readonly IBuildService _buildService;
    private readonly IStatsService _statsService;

    public MissionControlController(IBuildService buildService, IStatsService statsService)
    {
        _buildService = buildService;
        _statsService = statsService;
    }

    [HttpGet("stats/{projectId}")]
    public async Task<ActionResult<ProjectStats>> GetStats(string projectId)
    {
        var stats = await _statsService.GetProjectStatsAsync(projectId);
        return Ok(stats);
    }

    [HttpGet("pipelines/{projectId}")]
    public async Task<ActionResult<IEnumerable<Pipeline>>> GetPipelines(string projectId)
    {
        var pipelines = await _buildService.GetPipelinesAsync(projectId);
        return Ok(pipelines);
    }

    [HttpPost("pipelines/{projectId}")]
    public async Task<ActionResult<Pipeline>> QueuePipeline(string projectId, [FromQuery] string name)
    {
        var pipeline = await _buildService.QueuePipelineAsync(projectId, name);
        return Ok(pipeline);
    }
}
