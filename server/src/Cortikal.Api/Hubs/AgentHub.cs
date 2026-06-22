using Cortikal.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace Cortikal.Api.Hubs;

/// <summary>
/// Hub for real-time agent communication.
/// Streams messages between AI agents and the UI during "The Swarm" phase.
/// </summary>
public class AgentHub : Hub
{
    public async Task BroadcastAgentMessage(AgentMessage message)
    {
        await Clients.All.SendAsync("ReceiveAgentMessage", message);
    }
}
