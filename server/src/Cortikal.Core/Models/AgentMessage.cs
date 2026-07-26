namespace Cortikal.Core.Models;

public class AgentMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AgentRole { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Optional field if the message is associated with specific code/file changes.
    /// </summary>
    public string? CodeSnippet { get; set; }
    
    public MessageType Type { get; set; } = MessageType.Chat;
    
    public List<GeneratedFile>? GeneratedFiles { get; set; }
}

public enum MessageType
{
    Chat,
    Plan,
    Code,
    Review,
    Infrastructure,
    Error
}

public class GeneratedFile
{
    public string FilePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
