namespace Cortikal.Core.Interfaces;

public interface IVectorMemoryService
{
    /// <summary>
    /// Saves a memory chunk into the vector database.
    /// </summary>
    Task SaveMemoryAsync(string collection, string id, string text, string description);

    /// <summary>
    /// Searches the vector database for relevant memories based on a query.
    /// </summary>
    Task<IEnumerable<string>> SearchMemoryAsync(string collection, string query, int limit = 5);
}
