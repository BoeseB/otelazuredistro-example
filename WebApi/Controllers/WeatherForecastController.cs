using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class WeatherForecastController : ControllerBase
    {
        private static Counter<int> _weatherTypeCounter = DiagnosticsConfig.Meter.CreateCounter<int>("weather-summaries");

        private readonly ILogger<WeatherForecastController> _logger;

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await GetFiveDayForecast().ToArrayAsync();
        }

        private async IAsyncEnumerable<WeatherForecast> GetFiveDayForecast()
        {
            for (int day = 1; day <= 5; day++)
            {
                yield return await CalculateWeatherData(day);
            }
        }


        private async Task<WeatherForecast> CalculateWeatherData(int day)
        {
            // Create a Span tracing execution time of the contained code.
            using var activity = DiagnosticsConfig.ActivitySource.StartActivity();
            activity?.AddTag("day", day); // Add context to span. Call needs to be null safe as activity can be null if tracing is disabled.

            // Set dynamic context that is attached to every log contained in scope
            using var scope = _logger.BeginScope(new Dictionary<string, object>()
            {
                { "some.meta.info", "my context info" }
            });
            // Logging
            // DO NOT use string interpolation. Needs regex for analysis. Wastes process memory, because of new string every time.
            _logger.LogInformation($"Weather forecast for day {day}");
            // Ok. Better analysis. No memory pressure. Performance hit because parameters are boxed and unboxed in object type.
            _logger.LogInformation("Weather forecast for day {day}", day);
            // Optimized logging for performance critical parts
            LogWeatherForecast(day); 

            await Task.Delay(Random.Shared.Next(10, 30)); 
            var forecast = new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(day)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };

            // Count forecasts by weather type.
            _weatherTypeCounter.Add(1, new KeyValuePair<string, object?>("weather.type", forecast.Summary)); 
            return forecast;
        }

        /// <summary>
        /// Source generated logging code for performance optimized logging.
        /// Containing class needs to be partial. 
        /// </summary>
        [LoggerMessage(0, LogLevel.Information, "Weather forecast for day {day}")]
        private partial void LogWeatherForecast(int day);
    }
}