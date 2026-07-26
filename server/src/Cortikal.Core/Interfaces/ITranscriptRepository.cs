using Cortikal.Core.Models;

namespace Cortikal.Core.Interfaces;

public interface ITranscriptRepository
{
    void AddMessage(string projectId, AgentMessage message);
    IEnumerable<AgentMessage> GetTranscript(string projectId);
    void ClearTranscript(string projectId);
}
