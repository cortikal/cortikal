using System.Text.Json;
using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cortikal.Infrastructure.Data;

public class TranscriptRepository : ITranscriptRepository
{
    private readonly string _storageDir;
    private readonly ILogger<TranscriptRepository> _logger;
    private readonly object _lock = new();

    public TranscriptRepository(IHostEnvironment env, ILogger<TranscriptRepository> logger)
    {
        _logger = logger;
        
        var basePath = env.ContentRootPath;
        _storageDir = Path.Combine(basePath, ".data", "transcripts");
        
        Directory.CreateDirectory(_storageDir);
    }

    private string GetFilePath(string projectId) => Path.Combine(_storageDir, $"{projectId}.json");

    public void AddMessage(string projectId, AgentMessage message)
    {
        lock (_lock)
        {
            var messages = GetTranscript(projectId).ToList();
            messages.Add(message);
            Save(projectId, messages);
        }
    }

    public IEnumerable<AgentMessage> GetTranscript(string projectId)
    {
        lock (_lock)
        {
            var path = GetFilePath(projectId);
            if (!File.Exists(path))
            {
                return new List<AgentMessage>();
            }

            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<AgentMessage>>(json) ?? new List<AgentMessage>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load transcript for project {ProjectId}", projectId);
                return new List<AgentMessage>();
            }
        }
    }

    public void ClearTranscript(string projectId)
    {
        lock (_lock)
        {
            var path = GetFilePath(projectId);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private void Save(string projectId, List<AgentMessage> messages)
    {
        try
        {
            var path = GetFilePath(projectId);
            var json = JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save transcript for project {ProjectId}", projectId);
        }
    }
}
