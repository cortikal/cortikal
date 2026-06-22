using Microsoft.AspNetCore.SignalR;

namespace Cortikal.Api.Hubs;

/// <summary>
/// Hub for real-time orchestration state updates.
/// Used by "Mission Control" and "The Swarm" to track execution progress.
/// </summary>
public class OrchestratorHub : Hub
{
    public async Task SendStateUpdate(string state, string message)
    {
        // Broadcasts to all connected clients
        await Clients.All.SendAsync("ReceiveStateUpdate", state, message);
    }
}
