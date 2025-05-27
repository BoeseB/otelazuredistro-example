using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeatherFrontend.Services;

namespace WeatherFrontend.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly WeatherService _weatherService;

    public IndexModel(ILogger<IndexModel> logger, WeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    public SelectList? Cities { get; set; }
    
    [BindProperty]
    public string? SelectedCity { get; set; }

    public async Task OnGetAsync()
    {
        var cities = await _weatherService.GetSuggestedCitiesAsync();
        Cities = new SelectList(cities);
    }
}
