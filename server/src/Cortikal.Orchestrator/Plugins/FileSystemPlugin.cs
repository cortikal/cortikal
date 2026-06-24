using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;

namespace Cortikal.Orchestrator.Plugins;

public class FileSystemPlugin
{
    private readonly ILogger<FileSystemPlugin> _logger;

    public FileSystemPlugin(ILogger<FileSystemPlugin> logger)
    {
        _logger = logger;
    }

    [KernelFunction, Description("Reads the content of a file from the disk.")]
    public async Task<string> ReadFileAsync(
        [Description("The absolute or relative path to the file to read")] string filePath)
    {
        _logger.LogInformation("Agent reading file: {FilePath}", filePath);
        if (!File.Exists(filePath))
        {
            return $"Error: File not found at {filePath}";
        }
        return await File.ReadAllTextAsync(filePath);
    }

    [KernelFunction, Description("Writes content to a file on the disk, creating directories if needed.")]
    public async Task<string> WriteFileAsync(
        [Description("The path to the file to write")] string filePath,
        [Description("The content to write into the file")] string content)
    {
        _logger.LogInformation("Agent writing to file: {FilePath}", filePath);
        try
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            await File.WriteAllTextAsync(filePath, content);
            return $"Successfully wrote to {filePath}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write file {FilePath}", filePath);
            return $"Error writing file: {ex.Message}";
        }
    }
}
