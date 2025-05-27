using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherFrontend.Services;

namespace WeatherFrontend.Pages;

public class CurrentWeatherModel : PageModel
{
    private readonly ILogger<CurrentWeatherModel> _logger;
    private readonly WeatherService _weatherService;

    public CurrentWeatherModel(ILogger<CurrentWeatherModel> logger, WeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    [BindProperty(SupportsGet = true)]
    public string? City { get; set; }

    public WeatherForecast? Weather { get; private set; }
    
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(City))
        {
            Weather = await _weatherService.GetCurrentWeatherAsync(City);
            
            if (Weather == null)
            {
                ErrorMessage = $"Keine Wetterdaten für {City} gefunden.";
            }
        }
    }
}
