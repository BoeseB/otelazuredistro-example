using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true; // Log template Message with placeholders, this allows for easier queries of all similar logs
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: builder.Environment.ApplicationName)) // Optional: Add context
    .WithTracing(b => b
        .AddSource(DiagnosticsConfig.ApplicationName)) // Add Activity Source so traces are exported
    .WithMetrics(b => b
        .AddMeter(DiagnosticsConfig.ApplicationName)); // Add Meter so metrics are exported

var useApplicationInsights = !string.IsNullOrWhiteSpace(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
if (useApplicationInsights)
{
    builder.Services.AddOpenTelemetry().UseAzureMonitor(); // Add asp.net core instrumentation and exporter to Azure Application Insights
}

var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
if (useOtlpExporter)
{
    builder.Services.AddOpenTelemetry().UseOtlpExporter(); // Add OTLP exporter as second sink for demo multiple simultaneous sinks
}

// Register HttpClient and OpenWeatherClient
builder.Services.AddHttpClient<OpenWeatherClient>();
builder.Services.AddScoped<OpenWeatherClient>();

builder.Services.AddControllers();

// CORS-Konfiguration hinzufügen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("http://localhost:5207", "http://localhost:5220") // Anpassen an den Port der Blazor-App
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// CORS aktivieren
app.UseCors("AllowBlazorApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
