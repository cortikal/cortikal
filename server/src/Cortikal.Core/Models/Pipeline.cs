namespace Cortikal.Core.Models;

public enum BuildStatus
{
    Queued,
    Building,
    Testing,
    Deploying,
    Success,
    Failed,
    Cancelled
}

public class Pipeline
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public BuildStatus Status { get; set; } = BuildStatus.Queued;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    // Ordered list of stage logs
    public List<PipelineStage> Stages { get; set; } = new();
}

public class PipelineStage
{
    public string Name { get; set; } = string.Empty;
    public BuildStatus Status { get; set; } = BuildStatus.Queued;
    public string OutputLog { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
}

public class Deployment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PipelineId { get; set; } = string.Empty;
    public string Environment { get; set; } = "Production";
    public string Url { get; set; } = string.Empty;
    public DateTime DeployedAt { get; set; } = DateTime.UtcNow;
}
