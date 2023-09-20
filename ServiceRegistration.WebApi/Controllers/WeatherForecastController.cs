using Microsoft.AspNetCore.Mvc;
using ServiceRegistration.WebApi.Model;
using ServiceRegistration.WebApi.Services;

namespace ServiceRegistration.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private IWeatherForecastService ForecastService { get; }
    public WeatherForecastController(IWeatherForecastService forecastService)
    {
        ForecastService = forecastService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return ForecastService.Get();
    }
}