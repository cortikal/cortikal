using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cortikal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepository _repository;

    public ProjectController(IProjectRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Project>> GetProjects()
    {
        return Ok(_repository.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Project> GetProject(string id)
    {
        var project = _repository.GetById(id);
        if (project == null) return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public ActionResult<Project> CreateProject([FromBody] Project project)
    {
        project.Id = Guid.NewGuid().ToString();
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        
        _repository.Add(project);
        
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProject(string id)
    {
        var project = _repository.GetById(id);
        if (project == null) return NotFound();
        
        _repository.Delete(id);
        return NoContent();
    }
}
