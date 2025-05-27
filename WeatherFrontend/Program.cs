using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// Registriere HttpClient für die Kommunikation mit der WeatherAPI
builder.Services.AddHttpClient<WeatherFrontend.Services.WeatherService>(client =>
{
    var apiSettings = builder.Configuration.GetSection("WeatherApi");
    var baseUrl = apiSettings["BaseUrl"] ?? "http://localhost:5031";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddRazorPages();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();