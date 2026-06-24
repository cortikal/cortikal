namespace Cortikal.Core.Interfaces;

public class ProjectStats
{
    public int TotalTokensUsed { get; set; }
    public int SuccessfulBuilds { get; set; }
    public int FailedBuilds { get; set; }
    public double HealthScore { get; set; } // 0.0 to 100.0
}

public interface IStatsService
{
    /// <summary>
    /// Calculates the current health and statistics for a project.
    /// </summary>
    Task<ProjectStats> GetProjectStatsAsync(string projectId);
}
