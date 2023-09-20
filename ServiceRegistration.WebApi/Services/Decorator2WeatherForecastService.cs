using ServiceRegistration.WebApi.Model;

namespace ServiceRegistration.WebApi.Services;

[ServiceDecorator(typeof(Decorator1WeatherForecastService))]
public class Decorator2WeatherForecastService : IWeatherForecastService
{
    public IWeatherForecastService Service { get; }

    public Decorator2WeatherForecastService(IServiceDecorator<Decorator1WeatherForecastService> decorator)
    {
        Service = decorator.Implementation;
    }

    public IEnumerable<WeatherForecast> Get()
    {
        var forecasts = Service.Get();
        return forecasts.Skip(1).Take(2);
    }
}