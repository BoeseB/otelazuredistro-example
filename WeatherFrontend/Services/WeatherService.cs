using System.Text.Json;

namespace WeatherFrontend.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WeatherForecast?> GetCurrentWeatherAsync(string city)
    {
        _logger.LogInformation("Get current weather for {City}", city);
        try
        {
            return await _httpClient.GetFromJsonAsync<WeatherForecast>($"weatherforecast/current/{city}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current weather for {City}", city);
            return null;
        }
    }

    public async Task<IEnumerable<WeatherForecast>?> GetForecastAsync(string city)
    {
        _logger.LogInformation("Get weather forecast for {City}", city);
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>($"weatherforecast/forecast/{city}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forecast for {City}", city);
            return null;
        }
    }

    public async Task<IEnumerable<string>> GetSuggestedCitiesAsync()
    {
        // Diese Liste könnte dynamisch aus einer API abgerufen werden
        // Für dieses Beispiel verwenden wir eine vordefinierte Liste
        return await Task.FromResult(new List<string> 
        { 
            "Berlin", 
            "Hamburg", 
            "München", 
            "Köln", 
            "Frankfurt",
            "Stuttgart",
            "Düsseldorf",
            "Leipzig",
            "Dortmund",
            "Essen",
            "New York",
            "London",
            "Paris",
            "Tokyo",
            "Sydney"
        });
    }
}
