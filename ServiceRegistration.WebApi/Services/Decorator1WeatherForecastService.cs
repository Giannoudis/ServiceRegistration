using ServiceRegistration.WebApi.Model;

namespace ServiceRegistration.WebApi.Services;

[ServiceDecorator(typeof(WeatherForecastService))]
public class Decorator1WeatherForecastService : IWeatherForecastService
{
    public IWeatherForecastService Service { get; }

    public Decorator1WeatherForecastService(IServiceDecorator<WeatherForecastService> decorator)
    {
        Service = decorator.Implementation;
    }

    public IEnumerable<WeatherForecast> Get()
    {
        var forecasts = Service.Get();
        return forecasts.Skip(2).Take(3);
    }
}