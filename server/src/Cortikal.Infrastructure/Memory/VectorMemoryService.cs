using Cortikal.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cortikal.Infrastructure.Memory;

public class VectorMemoryService : IVectorMemoryService
{
    private readonly ILogger<VectorMemoryService> _logger;

    // For Phase 4 scaffold, we use a simple in-memory dictionary.
    // In production, this would wrap ISemanticTextMemory pointing to Qdrant, Milvus, or ChromaDB.
    private readonly Dictionary<string, List<MemoryRecord>> _collections = new();

    public VectorMemoryService(ILogger<VectorMemoryService> logger)
    {
        _logger = logger;
    }

    public Task SaveMemoryAsync(string collection, string id, string text, string description)
    {
        _logger.LogInformation("Saving memory to collection {Collection}: {Id}", collection, id);
        
        if (!_collections.ContainsKey(collection))
        {
            _collections[collection] = new List<MemoryRecord>();
        }

        var record = new MemoryRecord { Id = id, Text = text, Description = description };
        _collections[collection].RemoveAll(x => x.Id == id);
        _collections[collection].Add(record);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> SearchMemoryAsync(string collection, string query, int limit = 5)
    {
        _logger.LogInformation("Searching memory in collection {Collection} for query: {Query}", collection, query);

        if (!_collections.TryGetValue(collection, out var records))
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }

        // Mock semantic search: just return everything up to limit
        // In reality, this computes embedding for 'query' and does cosine similarity
        var results = records.Take(limit).Select(x => x.Text);
        
        return Task.FromResult(results);
    }

    private class MemoryRecord
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
