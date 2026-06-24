using Cortikal.Core.Enums;
using Cortikal.Core.Models;

namespace Cortikal.Core.Interfaces;

public interface IOrchestrator
{
    ExecutionState CurrentState { get; }
    
    /// <summary>
    /// Starts the orchestration process for a given architecture document.
    /// </summary>
    Task StartAsync(Project project, ArchDocument architecture);
    
    /// <summary>
    /// Pauses the current execution.
    /// </summary>
    Task PauseAsync();
    
    /// <summary>
    /// Resumes execution from the paused state.
    /// </summary>
    Task ResumeAsync();
    
    /// <summary>
    /// Cancels execution and moves to Idle/Error state.
    /// </summary>
    Task CancelAsync();
    
    /// <summary>
    /// Event fired when state changes.
    /// </summary>
    event EventHandler<StateChangedEventArgs>? StateChanged;
}

public class StateChangedEventArgs : EventArgs
{
    public ExecutionState OldState { get; set; }
    public ExecutionState NewState { get; set; }
    public string Message { get; set; } = string.Empty;
}
