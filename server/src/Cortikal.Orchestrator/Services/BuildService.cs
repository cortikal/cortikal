using System.Collections.Concurrent;
using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.Extensions.Logging;

namespace Cortikal.Orchestrator.Services;

public class BuildService : IBuildService
{
    private readonly ILogger<BuildService> _logger;
    private readonly ConcurrentDictionary<string, List<Pipeline>> _pipelines = new();

    public event EventHandler<Pipeline>? PipelineStateChanged;

    public BuildService(ILogger<BuildService> logger)
    {
        _logger = logger;
    }

    public async Task<Pipeline> QueuePipelineAsync(string projectId, string name)
    {
        var pipeline = new Pipeline
        {
            ProjectId = projectId,
            Name = name,
            Stages = new List<PipelineStage>
            {
                new() { Name = "Restore Dependencies" },
                new() { Name = "Build Source" },
                new() { Name = "Run Tests" },
                new() { Name = "Deploy" }
            }
        };

        var list = _pipelines.GetOrAdd(projectId, _ => new List<Pipeline>());
        list.Add(pipeline);

        _logger.LogInformation("Queued new pipeline {PipelineId} for project {ProjectId}", pipeline.Id, projectId);
        
        // Start background mock execution
        _ = Task.Run(() => RunMockPipelineAsync(pipeline));

        return await Task.FromResult(pipeline);
    }

    public Task<IEnumerable<Pipeline>> GetPipelinesAsync(string projectId)
    {
        if (_pipelines.TryGetValue(projectId, out var list))
        {
            return Task.FromResult<IEnumerable<Pipeline>>(list.OrderByDescending(p => p.StartedAt));
        }
        
        return Task.FromResult(Enumerable.Empty<Pipeline>());
    }

    private async Task RunMockPipelineAsync(Pipeline pipeline)
    {
        pipeline.Status = BuildStatus.Building;
        Notify(pipeline);

        foreach (var stage in pipeline.Stages)
        {
            stage.Status = BuildStatus.Building;
            Notify(pipeline);
            
            // Mock duration
            await Task.Delay(2000);

            stage.Status = BuildStatus.Success;
            stage.OutputLog = $"Success: {stage.Name} completed without errors.";
            stage.Duration = TimeSpan.FromSeconds(2);
            Notify(pipeline);
        }

        pipeline.Status = BuildStatus.Success;
        pipeline.CompletedAt = DateTime.UtcNow;
        Notify(pipeline);
    }

    private void Notify(Pipeline pipeline)
    {
        PipelineStateChanged?.Invoke(this, pipeline);
    }
}
