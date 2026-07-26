using Cortikal.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Cortikal.Orchestrator.Services;

public class LlmRouter : ILlmRouter
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LlmRouter> _logger;

    public LlmRouter(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<LlmRouter> logger)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Kernel GetKernelForTask(string taskType)
    {
        var builder = Kernel.CreateBuilder();
        builder.Services.AddSingleton<ILoggerFactory>(_serviceProvider.GetRequiredService<ILoggerFactory>());

        // 1. Determine provider from config
        var provider = _configuration[$"Cortikal:LLM:AgentRouting:{taskType}"] ?? "OpenAi";
        _logger.LogInformation("Routing task {TaskType} to provider {Provider}", taskType, provider);

        var apiKey = _configuration[$"Cortikal:LLM:{provider}:ApiKey"] ?? "";
        var modelId = _configuration[$"Cortikal:LLM:{provider}:Model"] ?? "";

        // 2. Add appropriate ChatCompletionService
        switch (provider.ToLowerInvariant())
        {
            case "openai":
                builder.AddOpenAIChatCompletion(modelId, apiKey);
                break;
            case "gemini":
                builder.AddGoogleAIGeminiChatCompletion(modelId, apiKey);
                break;
            case "claude":
                builder.Services.AddSingleton<IChatCompletionService>(new ClaudeChatCompletionService(apiKey, modelId));
                break;
            default:
                _logger.LogWarning("Unknown provider {Provider}, falling back to OpenAI", provider);
                builder.AddOpenAIChatCompletion(modelId, apiKey);
                break;
        }

        var kernel = builder.Build();

        // 3. Add Plugins
        // We resolve plugins from DI and add them to the kernel
        var filePlugin = _serviceProvider.GetService<Plugins.FileSystemPlugin>();
        if (filePlugin != null) kernel.Plugins.AddFromObject(filePlugin, "FileSystem");

        var gitPlugin = _serviceProvider.GetService<Plugins.GitPlugin>();
        if (gitPlugin != null) kernel.Plugins.AddFromObject(gitPlugin, "Git");

        var termPlugin = _serviceProvider.GetService<Plugins.TerminalPlugin>();
        if (termPlugin != null) kernel.Plugins.AddFromObject(termPlugin, "Terminal");

        return kernel;
    }
}
