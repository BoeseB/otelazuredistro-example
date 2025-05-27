using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherFrontend.Services;

namespace WeatherFrontend.Pages;

public class WeatherForecastModel : PageModel
{
    private readonly ILogger<WeatherForecastModel> _logger;
    private readonly WeatherService _weatherService;

    public WeatherForecastModel(ILogger<WeatherForecastModel> logger, WeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    [BindProperty(SupportsGet = true)]
    public string? City { get; set; }

    public IEnumerable<WeatherForecast>? Forecast { get; private set; }
    
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(City))
        {
            Forecast = await _weatherService.GetForecastAsync(City);
            
            if (Forecast == null || !Forecast.Any())
            {
                ErrorMessage = $"Keine Vorhersagedaten für {City} gefunden.";
            }
        }
    }
}
