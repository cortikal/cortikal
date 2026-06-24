using Cortikal.Core.Enums;
using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Microsoft.Extensions.Logging;

namespace Cortikal.Orchestrator.StateMachine;

public class OrchestratorStateMachine : IOrchestrator
{
    private readonly ILogger<OrchestratorStateMachine> _logger;
    private ExecutionState _currentState = ExecutionState.Idle;
    
    public ExecutionState CurrentState => _currentState;
    public event EventHandler<StateChangedEventArgs>? StateChanged;

    public OrchestratorStateMachine(ILogger<OrchestratorStateMachine> logger)
    {
        _logger = logger;
    }

    public async Task StartAsync(Project project, ArchDocument architecture)
    {
        if (_currentState != ExecutionState.Idle && _currentState != ExecutionState.Complete && _currentState != ExecutionState.Error)
        {
            _logger.LogWarning("Cannot start. Orchestrator is currently in {State}", _currentState);
            return;
        }

        await TransitionToAsync(ExecutionState.Planning, "Starting orchestration process. Analyzing architecture...");
        
        // This is a placeholder for the actual state machine loop.
        // It will eventually delegate to the Agent framework.
        _ = Task.Run(async () => await RunStateMachineLoop(project, architecture));
    }

    public async Task PauseAsync()
    {
        if (_currentState == ExecutionState.Idle || _currentState == ExecutionState.Complete || _currentState == ExecutionState.Error)
            return;

        await TransitionToAsync(ExecutionState.Paused, "Execution paused by user.");
    }

    public async Task ResumeAsync()
    {
        if (_currentState != ExecutionState.Paused)
            return;

        // Naive resume, normally we'd need to remember the previous active state
        await TransitionToAsync(ExecutionState.Generating, "Resuming execution.");
    }

    public async Task CancelAsync()
    {
        await TransitionToAsync(ExecutionState.Error, "Execution cancelled by user.");
    }

    private async Task RunStateMachineLoop(Project project, ArchDocument architecture)
    {
        try
        {
            // Simulate work for Phase 4 scaffold
            await Task.Delay(2000);
            
            if (_currentState == ExecutionState.Paused || _currentState == ExecutionState.Error) return;
            await TransitionToAsync(ExecutionState.Generating, "Architect has finished planning. Generating code...");
            
            await Task.Delay(3000);
            
            if (_currentState == ExecutionState.Paused || _currentState == ExecutionState.Error) return;
            await TransitionToAsync(ExecutionState.Reviewing, "Code generation complete. QA agent is reviewing...");

            await Task.Delay(2000);
            
            if (_currentState == ExecutionState.Paused || _currentState == ExecutionState.Error) return;
            await TransitionToAsync(ExecutionState.Complete, "Review passed. Project is ready.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in state machine loop.");
            await TransitionToAsync(ExecutionState.Error, $"Error: {ex.Message}");
        }
    }

    private Task TransitionToAsync(ExecutionState newState, string message)
    {
        var oldState = _currentState;
        _currentState = newState;
        
        _logger.LogInformation("Transitioning from {OldState} to {NewState}: {Message}", oldState, newState, message);
        
        StateChanged?.Invoke(this, new StateChangedEventArgs
        {
            OldState = oldState,
            NewState = newState,
            Message = message
        });

        return Task.CompletedTask;
    }
}
