using BlazorApplicationInsights;
using BlazorApplicationInsights.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WeatherUI;
using WeatherUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorApplicationInsights(config =>
{
    config.ConnectionString = "{Insert Connection String}";
},
async applicationInsights =>
{
    var telemetryItem = new TelemetryItem()
    {
        Tags = new Dictionary<string, object?>()
        {
            { "ai.cloud.role", "SPA" },
            { "ai.cloud.roleInstance", "Blazor Wasm" },
        }
    };

    await applicationInsights.AddTelemetryInitializer(telemetryItem);
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<WeatherService>();

await builder.Build().RunAsync();
