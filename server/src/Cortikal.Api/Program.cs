using Cortikal.Api.Hubs;
using Cortikal.ArchParser;
using Cortikal.Core.Interfaces;
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
builder.Services.AddSingleton<IOrchestrator, Cortikal.Orchestrator.StateMachine.OrchestratorStateMachine>();
builder.Services.AddSingleton<IBuildService, Cortikal.Orchestrator.Services.BuildService>();
builder.Services.AddSingleton<IStatsService, Cortikal.Orchestrator.Services.StatsService>();
builder.Services.AddHostedService<Cortikal.Api.Services.OrchestratorEventService>();

// CORS policy for Next.js frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3005", "http://localhost:3100") // Next.js ports
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

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("FrontendPolicy");

app.UseAuthorization();

// Map REST API Controllers
app.MapControllers();

// Map SignalR Hubs
app.MapHub<OrchestratorHub>("/hubs/orchestrator");
app.MapHub<AgentHub>("/hubs/agent");

app.Run();
