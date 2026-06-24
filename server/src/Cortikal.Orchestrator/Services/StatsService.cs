using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;

namespace Cortikal.Orchestrator.Services;

public class StatsService : IStatsService
{
    private readonly IBuildService _buildService;

    public StatsService(IBuildService buildService)
    {
        _buildService = buildService;
    }

    public async Task<ProjectStats> GetProjectStatsAsync(string projectId)
    {
        var pipelines = await _buildService.GetPipelinesAsync(projectId);
        
        int success = pipelines.Count(p => p.Status == BuildStatus.Success);
        int failed = pipelines.Count(p => p.Status == BuildStatus.Failed);
        int total = success + failed;

        double health = 100.0;
        if (total > 0)
        {
            health = ((double)success / total) * 100.0;
        }

        // Mock token usage based on arbitrary logic for now
        int tokens = 154200 + (pipelines.Count() * 1250);

        return new ProjectStats
        {
            TotalTokensUsed = tokens,
            SuccessfulBuilds = success,
            FailedBuilds = failed,
            HealthScore = health
        };
    }
}
