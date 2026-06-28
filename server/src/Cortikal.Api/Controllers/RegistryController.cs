using Microsoft.AspNetCore.Mvc;

namespace Cortikal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistryController : ControllerBase
{
    private readonly string _registryPath;

    public RegistryController(IConfiguration configuration)
    {
        // For development, navigate from server/src/Cortikal.Api to registry/templates
        var basePath = AppContext.BaseDirectory;
        
        // This is a bit hacky for dev. In prod it would be configured.
        _registryPath = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", "..", "..", "registry", "templates"));
    }

    [HttpGet("templates")]
    public ActionResult<IEnumerable<TemplateSummary>> GetTemplates()
    {
        if (!Directory.Exists(_registryPath))
        {
            return NotFound($"Registry path not found: {_registryPath}");
        }

        var templates = new List<TemplateSummary>();
        var dirs = Directory.GetDirectories(_registryPath);

        foreach (var dir in dirs)
        {
            var dirName = Path.GetFileName(dir);
            var readmePath = Path.Combine(dir, "README.md");
            var archPath = Path.Combine(dir, "arch.md");
            
            if (System.IO.File.Exists(archPath))
            {
                // In a real scenario, we might parse the YAML frontmatter of arch.md to get title/description
                templates.Add(new TemplateSummary
                {
                    Id = dirName,
                    Name = dirName.Replace("-", " "),
                    Description = System.IO.File.Exists(readmePath) ? System.IO.File.ReadAllLines(readmePath).FirstOrDefault(l => !l.StartsWith("#"))?.Trim() ?? "No description" : "No description",
                    HasPreview = System.IO.File.Exists(Path.Combine(dir, "preview.png"))
                });
            }
        }

        return Ok(templates);
    }

    [HttpGet("templates/{id}/content")]
    public ActionResult<string> GetTemplateContent(string id)
    {
        // Map frontend quick start template IDs to registry folder names
        if (id == "web-app")
        {
            id = "web-app-basic";
        }

        var archPath = Path.Combine(_registryPath, id, "arch.md");
        if (!System.IO.File.Exists(archPath))
        {
            return NotFound($"Template {id} not found.");
        }

        var content = System.IO.File.ReadAllText(archPath);
        return Ok(content);
    }
}

public class TemplateSummary
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool HasPreview { get; set; }
}
