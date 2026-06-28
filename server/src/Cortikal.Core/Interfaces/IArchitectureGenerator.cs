using Cortikal.Core.Models;

namespace Cortikal.Core.Interfaces;

/// <summary>
/// Generates an ArchDocument from a natural language prompt using an LLM.
/// </summary>
public interface IArchitectureGenerator
{
    Task<ArchParseResult> GenerateFromPromptAsync(string prompt, CancellationToken cancellationToken = default);
}
