using System.Net.Http.Json;

namespace WeatherUI.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl = "http://localhost:5031/";

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Die Basis-URL auf die WebAPI setzen
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
            // Im Produktivcode sollte hier geloggt werden
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
            // Im Produktivcode sollte hier geloggt werden
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
