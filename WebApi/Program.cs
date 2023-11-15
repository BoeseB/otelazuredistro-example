using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: builder.Environment.ApplicationName)) // Optional: Add context
    .WithTracing(b => b
        .AddSource(DiagnosticsConfig.ApplicationName)) // Add Activity Source so traces are exported
    .WithMetrics(b => b
        .AddMeter(DiagnosticsConfig.ApplicationName)) // Add Meter so metrics are exported
    .UseAzureMonitor(); // Add asp.net core instrumentation and exporter to Azure Application Insights

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();