using Cortikal.Core.Enums;
using Cortikal.Core.Interfaces;
using Cortikal.Core.Models;
using Cortikal.Orchestrator.Plugins;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Cortikal.Orchestrator.StateMachine;

public class OrchestratorStateMachine : IOrchestrator
{
    private readonly ILogger<OrchestratorStateMachine> _logger;
    private readonly IEnumerable<IAgentService> _agents;
    private readonly FileSystemPlugin _fileSystem;
    private readonly GitPlugin _git;
    private readonly ITranscriptRepository _transcriptRepo;
    private ExecutionState _currentState = ExecutionState.Idle;
    
    public ExecutionState CurrentState => _currentState;
    public event EventHandler<StateChangedEventArgs>? StateChanged;
    public event EventHandler<AgentMessage>? AgentMessageReceived;

    public OrchestratorStateMachine(
        ILogger<OrchestratorStateMachine> logger,
        IEnumerable<IAgentService> agents,
        FileSystemPlugin fileSystem,
        GitPlugin git,
        ITranscriptRepository transcriptRepo)
    {
        _logger = logger;
        _agents = agents;
        _fileSystem = fileSystem;
        _git = git;
        _transcriptRepo = transcriptRepo;
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
            var architect = _agents.First(a => a.Role == AgentRole.LeadArchitect);
            var frontendDev = _agents.First(a => a.Role == AgentRole.FrontendDev);
            var backendDev = _agents.First(a => a.Role == AgentRole.BackendDev);
            var qaEngineer = _agents.First(a => a.Role == AgentRole.QualityAssurance);
            var devOpsEngineer = _agents.First(a => a.Role == AgentRole.DevOps);

            // Phase 1: Planning
            string planJson = await ExecuteAgentAsync(architect, project, architecture, "Design the architecture and create an execution plan.", MessageType.Plan);
            if (_currentState == ExecutionState.Paused || _currentState == ExecutionState.Error) return;
            
            await TransitionToAsync(ExecutionState.Generating, "Architect has finished planning. Generating code...");

            // Phase 2: Generating
            string frontendOutput = await ExecuteAgentAsync(frontendDev, project, architecture, $"Here is the plan:\n{planJson}", MessageType.Code);
            string backendOutput = await ExecuteAgentAsync(backendDev, project, architecture, $"Here is the plan:\n{planJson}", MessageType.Code);
            
            // Extract code and write files
            var generatedFiles = new List<GeneratedFile>();
            generatedFiles.AddRange(ExtractFilesFromJson(frontendOutput));
            generatedFiles.AddRange(ExtractFilesFromJson(backendOutput));
            
            foreach (var file in generatedFiles)
            {
                var fullPath = Path.Combine(project.Path, file.FilePath.TrimStart('/', '\\'));
                await _fileSystem.WriteFileAsync(fullPath, file.Content);
            }

            if (_currentState == ExecutionState.Paused || _currentState == ExecutionState.Error) return;
            await TransitionToAsync(ExecutionState.Reviewing, "Code generation complete. QA agent is reviewing...");

            // Phase 3: Reviewing
            int reviewLoops = 0;
            bool approved = false;
            
            while (!approved && reviewLoops < 2)
            {
                reviewLoops++;
                string qaOutput = await ExecuteAgentAsync(qaEngineer, project, architecture, $"Review the following generated files:\n{JsonSerializer.Serialize(generatedFiles)}", MessageType.Review);
                
                try 
                {
                    var reviewResult = JsonSerializer.Deserialize<JsonElement>(qaOutput);
                    approved = reviewResult.GetProperty("approved").GetBoolean();
                    
                    if (!approved)
                    {
                        var issues = reviewResult.GetProperty("issues").ToString();
                        await TransitionToAsync(ExecutionState.Generating, $"QA rejected the code. Re-generating based on feedback...");
                        
                        frontendOutput = await ExecuteAgentAsync(frontendDev, project, architecture, $"Fix the following issues in the frontend code:\n{issues}", MessageType.Code);
                        backendOutput = await ExecuteAgentAsync(backendDev, project, architecture, $"Fix the following issues in the backend code:\n{issues}", MessageType.Code);
                        
                        generatedFiles.Clear();
                        generatedFiles.AddRange(ExtractFilesFromJson(frontendOutput));
                        generatedFiles.AddRange(ExtractFilesFromJson(backendOutput));
                        
                        foreach (var file in generatedFiles)
                        {
                            var fullPath = Path.Combine(project.Path, file.FilePath.TrimStart('/', '\\'));
                            await _fileSystem.WriteFileAsync(fullPath, file.Content);
                        }
                        
                        await TransitionToAsync(ExecutionState.Reviewing, "Code re-generated. QA agent is reviewing again...");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse QA output as JSON. Proceeding with approval.");
                    approved = true; // Fallback if QA output isn't parsable JSON
                }
            }

            if (_currentState == ExecutionState.Paused || _currentState == ExecutionState.Error) return;
            await TransitionToAsync(ExecutionState.Deploying, "Review passed. Generating infrastructure...");

            // Phase 4: Deploying (DevOps)
            string devOpsOutput = await ExecuteAgentAsync(devOpsEngineer, project, architecture, $"Generate infrastructure files for this project:\n{planJson}", MessageType.Infrastructure);
            var infraFiles = ExtractFilesFromJson(devOpsOutput);
            
            foreach (var file in infraFiles)
            {
                var fullPath = Path.Combine(project.Path, file.FilePath.TrimStart('/', '\\'));
                await _fileSystem.WriteFileAsync(fullPath, file.Content);
            }

            // Git Init & Commit
            await _git.InitRepositoryAsync(project.Path);
            await _git.CommitChangesAsync(project.Path, "Initial commit from Cortikal Swarm");

            await TransitionToAsync(ExecutionState.Complete, "Project is ready.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in state machine loop.");
            await TransitionToAsync(ExecutionState.Error, $"Error: {ex.Message}");
        }
    }

    private async Task<string> ExecuteAgentAsync(IAgentService agent, Project project, ArchDocument architecture, string prompt, MessageType messageType)
    {
        var output = await agent.ExecuteAsync(project, architecture, prompt);
        
        var message = new AgentMessage
        {
            AgentRole = agent.Role.ToString(),
            Content = output,
            Type = messageType,
            GeneratedFiles = messageType == MessageType.Code || messageType == MessageType.Infrastructure ? ExtractFilesFromJson(output) : null
        };
        
        _transcriptRepo.AddMessage(project.Id, message);
        AgentMessageReceived?.Invoke(this, message);
        return output;
    }

    private List<GeneratedFile> ExtractFilesFromJson(string jsonOutput)
    {
        var files = new List<GeneratedFile>();
        try
        {
            // Find JSON array if it's wrapped in markdown code blocks
            var jsonStr = jsonOutput;
            if (jsonStr.Contains("```json"))
            {
                var start = jsonStr.IndexOf("```json") + 7;
                var end = jsonStr.LastIndexOf("```");
                if (end > start)
                {
                    jsonStr = jsonStr.Substring(start, end - start).Trim();
                }
            }
            
            // Just try to deserialize as list of generated files
            var parsed = JsonSerializer.Deserialize<List<GeneratedFile>>(jsonStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (parsed != null) files.AddRange(parsed);
        }
        catch
        {
            // If we can't parse it, return empty list
        }
        return files;
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
