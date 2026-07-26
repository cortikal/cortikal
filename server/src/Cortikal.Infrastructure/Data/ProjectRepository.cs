using System.Text.Json;
using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cortikal.Infrastructure.Data;

public class ProjectRepository : IProjectRepository
{
    private readonly string _storagePath;
    private readonly ILogger<ProjectRepository> _logger;
    private readonly object _lock = new();

    public ProjectRepository(IHostEnvironment env, ILogger<ProjectRepository> logger)
    {
        _logger = logger;
        
        // Store in a .data directory in the server root
        var basePath = env.ContentRootPath;
        _storagePath = Path.Combine(basePath, ".data", "projects.json");
        
        Directory.CreateDirectory(Path.GetDirectoryName(_storagePath)!);
    }

    private List<Project> Load()
    {
        lock (_lock)
        {
            if (!File.Exists(_storagePath))
            {
                return new List<Project>();
            }

            try
            {
                var json = File.ReadAllText(_storagePath);
                return JsonSerializer.Deserialize<List<Project>>(json) ?? new List<Project>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load projects from {Path}", _storagePath);
                return new List<Project>();
            }
        }
    }

    private void Save(List<Project> projects)
    {
        lock (_lock)
        {
            try
            {
                var json = JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_storagePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save projects to {Path}", _storagePath);
            }
        }
    }

    public IEnumerable<Project> GetAll()
    {
        return Load().OrderByDescending(p => p.UpdatedAt);
    }

    public Project? GetById(string id)
    {
        return Load().FirstOrDefault(p => p.Id == id);
    }

    public void Add(Project project)
    {
        var projects = Load();
        projects.Add(project);
        Save(projects);
    }

    public void Update(Project project)
    {
        var projects = Load();
        var index = projects.FindIndex(p => p.Id == project.Id);
        if (index != -1)
        {
            projects[index] = project;
            Save(projects);
        }
    }

    public void Delete(string id)
    {
        var projects = Load();
        var initialCount = projects.Count;
        projects.RemoveAll(p => p.Id == id);
        if (projects.Count != initialCount)
        {
            Save(projects);
        }
    }
}
