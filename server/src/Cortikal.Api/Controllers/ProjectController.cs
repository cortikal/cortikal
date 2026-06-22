using Cortikal.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cortikal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private static readonly List<Project> _projects = new(); // In-memory for now

    [HttpGet]
    public ActionResult<IEnumerable<Project>> GetProjects()
    {
        return Ok(_projects);
    }

    [HttpGet("{id}")]
    public ActionResult<Project> GetProject(string id)
    {
        var project = _projects.FirstOrDefault(p => p.Id == id);
        if (project == null) return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public ActionResult<Project> CreateProject([FromBody] Project project)
    {
        project.Id = Guid.NewGuid().ToString();
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        _projects.Add(project);
        
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProject(string id)
    {
        var project = _projects.FirstOrDefault(p => p.Id == id);
        if (project == null) return NotFound();
        
        _projects.Remove(project);
        return NoContent();
    }
}
