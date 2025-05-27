using System.Net.Http.Json;

namespace WeatherUI.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    private readonly string _apiBaseUrl = "http://localhost:5031/";

    public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(_apiBaseUrl);
    }

    public async Task<WeatherForecast?> GetCurrentWeatherAsync(string city)
    {
        if (string.IsNullOrEmpty(city))
            return null;

        try
        {
            return await _httpClient.GetFromJsonAsync<WeatherForecast>($"WeatherForecast/current/{city}");
        }
        catch (Exception)
        {
            _logger.LogError("Fehler beim Abrufen der aktuellen Wetterdaten für {city}", city);
            return null;
        }
    }

    public async Task<IEnumerable<WeatherForecast>?> GetForecastAsync(string city)
    {
        if (string.IsNullOrEmpty(city))
            return null;

        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>($"WeatherForecast/forecast/{city}");
        }
        catch (Exception)
        {
            _logger.LogError("Fehler beim Abrufen der Wettervorhersage für {city}", city);
            return null;
        }
    }
}

public class WeatherForecast
{
    public string? City { get; set; }
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}
