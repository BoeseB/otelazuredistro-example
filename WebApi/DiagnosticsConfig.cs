using System.Diagnostics;
using System.Diagnostics.Metrics;

static class DiagnosticsConfig
{
    public static readonly string ApplicationName = "MyCompany.WeatherApi";

    public static readonly ActivitySource ActivitySource = new(ApplicationName, "1.0.0");

    public static readonly Meter Meter = new (ApplicationName, "1.0.0");
}