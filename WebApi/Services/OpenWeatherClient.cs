using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace WebApi.Services;

public class OpenWeatherClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenWeatherClient> _logger;
    private readonly string _apiKey;

    public OpenWeatherClient(HttpClient httpClient, ILogger<OpenWeatherClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_APIKEY") 
            ?? throw new InvalidOperationException("OpenWeather API key not found");
        
        _httpClient.BaseAddress = new Uri("https://api.openweathermap.org/");
    }

    public async Task<CurrentWeatherResponse?> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken = default)
    {
        try
        {
            using var activity = DiagnosticsConfig.ActivitySource.StartActivity("Get current weather");
            activity?.AddTag("city", city);
            
            var requestUrl = $"data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetFromJsonAsync<CurrentWeatherResponse>(requestUrl, cancellationToken);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current weather for city {City}", city);
            throw;
        }
    }

    public async Task<ForecastResponse?> GetForecastAsync(string city, CancellationToken cancellationToken = default)
    {
        try
        {
            using var activity = DiagnosticsConfig.ActivitySource.StartActivity("Get weather forecast");
            activity?.AddTag("city", city);
            
            var requestUrl = $"data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetFromJsonAsync<ForecastResponse>(requestUrl, cancellationToken);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forecast for city {City}", city);
            throw;
        }
    }
}

public class CurrentWeatherResponse
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("main")]
    public MainWeatherInfo? Main { get; set; }
    
    [JsonPropertyName("weather")]
    public WeatherCondition[]? Weather { get; set; }
}

public class MainWeatherInfo
{
    [JsonPropertyName("temp")]
    public float Temperature { get; set; }
    
    [JsonPropertyName("feels_like")]
    public float FeelsLike { get; set; }
    
    [JsonPropertyName("temp_min")]
    public float MinTemperature { get; set; }
    
    [JsonPropertyName("temp_max")]
    public float MaxTemperature { get; set; }
    
    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class WeatherCondition
{
    [JsonPropertyName("main")]
    public string? Main { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
}

public class ForecastResponse
{
    [JsonPropertyName("list")]
    public ForecastItem[]? List { get; set; }
    
    [JsonPropertyName("city")]
    public CityInfo? City { get; set; }
}

public class ForecastItem
{
    [JsonPropertyName("dt")]
    public long Timestamp { get; set; }
    
    [JsonPropertyName("main")]
    public MainWeatherInfo? Main { get; set; }
    
    [JsonPropertyName("weather")]
    public WeatherCondition[]? Weather { get; set; }
    
    [JsonPropertyName("dt_txt")]
    public string? DateTimeText { get; set; }
}

public class CityInfo
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("country")]
    public string? Country { get; set; }
}
