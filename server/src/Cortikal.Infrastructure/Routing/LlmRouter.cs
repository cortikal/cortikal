using Cortikal.Core.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cortikal.Infrastructure.Routing;

public class LlmRouter : ILlmRouter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LlmRouter> _logger;

    public LlmRouter(IServiceProvider serviceProvider, ILogger<LlmRouter> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Kernel GetKernelForTask(string taskType)
    {
        _logger.LogInformation("Routing task type '{TaskType}' to optimal LLM provider.", taskType);
        
        // In a real implementation, we would inspect taskType 
        // (e.g. "complex_architecture", "simple_ui", "code_review")
        // and resolve a Kernel configured for OpenAI GPT-4o, Anthropic Claude 3.5 Sonnet, 
        // or a local Ollama model respectively.
        
        // For the Phase 4 scaffold, we resolve the default scoped Kernel.
        // The DI container should be configured to provide a Kernel.
        var builder = Kernel.CreateBuilder();
        
        // Mock fallback using standard AI settings if present, otherwise just an empty kernel
        // builder.AddOpenAIChatCompletion("gpt-4o", "API_KEY");
        
        return builder.Build();
    }
}
