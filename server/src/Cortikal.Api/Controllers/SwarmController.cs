using Cortikal.Core.Enums;
using Cortikal.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cortikal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SwarmController : ControllerBase
{
    private readonly IOrchestrator _orchestrator;
    private readonly IProjectRepository _projectRepository;
    private readonly IArchParser _archParser;

    public SwarmController(IOrchestrator orchestrator, IProjectRepository projectRepository, IArchParser archParser)
    {
        _orchestrator = orchestrator;
        _projectRepository = projectRepository;
        _archParser = archParser;
    }

    [HttpPost("start/{projectId}")]
    public async Task<IActionResult> StartSwarm(string projectId, [FromBody] StartSwarmRequest request)
    {
        var project = _projectRepository.GetById(projectId);
        if (project == null) return NotFound("Project not found.");

        if (_orchestrator.CurrentState != ExecutionState.Idle && _orchestrator.CurrentState != ExecutionState.Complete && _orchestrator.CurrentState != ExecutionState.Error)
        {
            return BadRequest("Swarm is already running.");
        }

        var archResult = _archParser.Parse(request.ArchMdContent);
        if (archResult.Document == null)
        {
            return BadRequest($"Failed to parse arch.md: {string.Join(", ", archResult.Errors)}");
        }

        // Fire and forget (in a real app, use a background task queue)
        _ = _orchestrator.StartAsync(project, archResult.Document);

        return Accepted();
    }

    [HttpPost("pause")]
    public async Task<IActionResult> PauseSwarm()
    {
        await _orchestrator.PauseAsync();
        return Ok();
    }

    [HttpPost("resume")]
    public async Task<IActionResult> ResumeSwarm()
    {
        await _orchestrator.ResumeAsync();
        return Ok();
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> CancelSwarm()
    {
        await _orchestrator.CancelAsync();
        return Ok();
    }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        return Ok(new { State = _orchestrator.CurrentState.ToString() });
    }
}

public class StartSwarmRequest
{
    public string ArchMdContent { get; set; } = string.Empty;
}
