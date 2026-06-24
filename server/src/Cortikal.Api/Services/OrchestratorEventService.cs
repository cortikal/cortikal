using Cortikal.Api.Hubs;
using Cortikal.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Cortikal.Api.Services;

/// <summary>
/// Listens to events from the core Orchestrator and broadcasts them
/// to connected frontend clients via SignalR Hubs.
/// </summary>
public class OrchestratorEventService : IHostedService
{
    private readonly IOrchestrator _orchestrator;
    private readonly IHubContext<OrchestratorHub> _orchestratorHub;

    public OrchestratorEventService(IOrchestrator orchestrator, IHubContext<OrchestratorHub> orchestratorHub)
    {
        _orchestrator = orchestrator;
        _orchestratorHub = orchestratorHub;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _orchestrator.StateChanged += OnStateChanged;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _orchestrator.StateChanged -= OnStateChanged;
        return Task.CompletedTask;
    }

    private void OnStateChanged(object? sender, StateChangedEventArgs e)
    {
        // Broadcast the state transition to all Mission Control / Swarm clients
        _orchestratorHub.Clients.All.SendAsync("ReceiveStateUpdate", e.NewState.ToString(), e.Message);
    }
}
