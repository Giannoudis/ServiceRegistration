using ServiceRegistration.WebApi.Model;

namespace ServiceRegistration.WebApi.Services;

// decorated by code
public class Decorator3WeatherForecastService : IWeatherForecastService
{
    public IWeatherForecastService Service { get; }

    public Decorator3WeatherForecastService(IServiceDecorator<Decorator2WeatherForecastService> decorator)
    {
        Service = decorator.Implementation;
    }

    public IEnumerable<WeatherForecast> Get()
    {
        var forecasts = Service.Get();
        return forecasts.Take(1);
    }
}