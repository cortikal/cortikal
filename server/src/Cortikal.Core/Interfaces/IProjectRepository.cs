using Cortikal.Core.Models;

namespace Cortikal.Core.Interfaces;

public interface IProjectRepository
{
    IEnumerable<Project> GetAll();
    Project? GetById(string id);
    void Add(Project project);
    void Update(Project project);
    void Delete(string id);
}
