using Cortikal.Api.Hubs;
using Cortikal.Api.Services;
using Cortikal.ArchParser;
using Cortikal.Core.Interfaces;
using Cortikal.Infrastructure.Generation;
using Cortikal.Orchestrator.Agents;
using Cortikal.Orchestrator.Plugins;
using Cortikal.Orchestrator.Services;
using Cortikal.Orchestrator.StateMachine;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();

// Register Cortikal Services
builder.Services.AddSingleton<IArchParser, ArchMarkdownParser>();
builder.Services.AddSingleton<IProjectRepository, Cortikal.Infrastructure.Data.ProjectRepository>();
builder.Services.AddSingleton<IOrchestrator, OrchestratorStateMachine>();
builder.Services.AddSingleton<IBuildService, BuildService>();
builder.Services.AddSingleton<IStatsService, StatsService>();
builder.Services.AddHostedService<OrchestratorEventService>();

// Register Plugins
builder.Services.AddSingleton<FileSystemPlugin>();
builder.Services.AddSingleton<GitPlugin>();
builder.Services.AddSingleton<TerminalPlugin>();

// Register LLM Router
builder.Services.AddSingleton<ILlmRouter, LlmRouter>();

// Register Agents
builder.Services.AddSingleton<IAgentService, ArchitectAgent>();
builder.Services.AddSingleton<IAgentService, FrontendDevAgent>();
builder.Services.AddSingleton<IAgentService, BackendDevAgent>();
builder.Services.AddSingleton<IAgentService, QAEngineerAgent>();
builder.Services.AddSingleton<IAgentService, DevOpsEngineerAgent>();

// Architecture generator — uses a typed HttpClient for OpenAI calls
builder.Services.AddHttpClient<IArchitectureGenerator, ArchitectureGeneratorService>();

// CORS policy for Next.js frontend — origins configurable via Cortikal:Frontend:Origins
var corsOrigins = builder.Configuration.GetSection("Cortikal:Frontend:Origins").Get<string[]>()
    ?? ["http://localhost:3000", "http://localhost:3005", "http://localhost:3100"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection(); // Disabled to prevent local HTTPS self-signed cert issues

// Enable CORS
app.UseCors("FrontendPolicy");

app.UseAuthorization();

// Map REST API Controllers
app.MapControllers();

// Map SignalR Hubs
app.MapHub<OrchestratorHub>("/hubs/orchestrator");
app.MapHub<AgentHub>("/hubs/agent");

app.Run();
