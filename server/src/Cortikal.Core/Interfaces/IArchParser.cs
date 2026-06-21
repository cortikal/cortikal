using Cortikal.Core.Models;

namespace Cortikal.Core.Interfaces;

/// <summary>
/// Interface for parsing arch.md files into ArchDocument models.
/// </summary>
public interface IArchParser
{
    /// <summary>
    /// Parse an arch.md file content string.
    /// </summary>
    ArchParseResult Parse(string content);

    /// <summary>
    /// Serialize an ArchDocument back to arch.md format.
    /// </summary>
    string Serialize(ArchDocument document);
}

/// <summary>
/// Result of parsing an arch.md file.
/// </summary>
public class ArchParseResult
{
    public bool Success { get; set; }
    public ArchDocument? Document { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}
