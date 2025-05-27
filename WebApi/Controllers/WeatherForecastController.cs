using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        OpenWeatherClient openWeatherClient) : ControllerBase
    {
        private static readonly Counter<int> WeatherCityCounter = DiagnosticsConfig.Meter.CreateCounter<int>("weather-requests-by-city");

        [HttpGet("current/{city}")]
        public async Task<ActionResult<WeatherForecast>> GetCurrent(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City name is required");
            }

            try
            {
                var weatherData = await openWeatherClient.GetCurrentWeatherAsync(city);
                if (weatherData == null)
                {
                    return NotFound($"Weather data for {city} not found");
                }

                var forecast = new WeatherForecast
                {
                    City = weatherData.Name,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    TemperatureC = (int)Math.Round(weatherData.Main?.Temperature ?? 0),
                    Summary = weatherData.Weather?.FirstOrDefault()?.Main ?? "Unknown"
                };

                // Count forecasts by city
                WeatherCityCounter.Add(1, new KeyValuePair<string, object?>("weather.city", city));

                return forecast;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting current weather for {City}", city);
                return StatusCode(500, "Error retrieving weather data");
            }
        }

        [HttpGet("forecast/{city}")]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetRealForecast(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City name is required");
            }

            try
            {
                var forecastData = await openWeatherClient.GetForecastAsync(city);
                if (forecastData == null || forecastData.List == null || forecastData.List.Length == 0)
                {
                    return NotFound($"Forecast data for {city} not found");
                }

                // Group by date to get daily forecasts (OpenWeather returns data every 3 hours)
                var dailyForecasts = forecastData.List
                    .GroupBy(item => DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(item.Timestamp).DateTime))
                    .Take(5) // Take 5 days
                    .Select(group =>
                    {
                        // Get the noon forecast if available, otherwise take the first one of the day
                        var dayForecast = group.FirstOrDefault(item => 
                            DateTimeOffset.FromUnixTimeSeconds(item.Timestamp).Hour is >= 11 and <= 13) 
                            ?? group.First();

                        return new WeatherForecast
                        {
                            City = forecastData.City?.Name,
                            Date = group.Key,
                            TemperatureC = (int)Math.Round(dayForecast.Main?.Temperature ?? 0),
                            Summary = dayForecast.Weather?.FirstOrDefault()?.Main ?? "Unknown"
                        };
                    })
                    .ToList();

                // Count forecasts by city
                WeatherCityCounter.Add(1, new KeyValuePair<string, object?>("weather.city", city));

                return dailyForecasts;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting forecast for {City}", city);
                return StatusCode(500, "Error retrieving forecast data");
            }
        }

        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet("random/{city}")]
        public async Task<IEnumerable<WeatherForecast>> Get(string city)
        {
            return await GetFiveDayForecast(city).ToArrayAsync();
        }
        
        private async IAsyncEnumerable<WeatherForecast> GetFiveDayForecast(string city)
        {
            for (int day = 1; day <= 5; day++)
            {
                yield return await CalculateWeatherData(city, day);
            }
        }

        private async Task<WeatherForecast> CalculateWeatherData(string city, int day)
        {
            // Create a Span tracing execution time of the contained code.
            using var activity = DiagnosticsConfig.ActivitySource.StartActivity();
            activity?.AddTag("day", day); // Add context to span. Call needs to be null safe as activity can be null if tracing is disabled.
            activity?.AddTag("city", city);

            LogWeatherForecast(city, day); 

            await Task.Delay(Random.Shared.Next(10, 30)); 
            var forecast = new WeatherForecast
            {
                City = city,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(day)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };

            // Count forecasts by city
            WeatherCityCounter.Add(1, new KeyValuePair<string, object?>("weather.city", city));
            return forecast;
        }

        /// <summary>
        /// Source generated logging code for performance optimized logging.
        /// Containing class needs to be partial. 
        /// </summary>
        [LoggerMessage(0, LogLevel.Information, "Weather forecast for day {day}")]
        private partial void LogWeatherForecast(string city, int day);
    }
}
