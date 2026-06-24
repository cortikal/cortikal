using Cortikal.Core.Models;

namespace Cortikal.Core.Interfaces;

public interface IBuildService
{
    /// <summary>
    /// Queues a new pipeline execution for a project.
    /// </summary>
    Task<Pipeline> QueuePipelineAsync(string projectId, string name);

    /// <summary>
    /// Gets all pipelines for a specific project.
    /// </summary>
    Task<IEnumerable<Pipeline>> GetPipelinesAsync(string projectId);

    /// <summary>
    /// Event fired when a pipeline's state changes.
    /// </summary>
    event EventHandler<Pipeline>? PipelineStateChanged;
}
