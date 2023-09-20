using ServiceRegistration.WebApi.Model;

namespace ServiceRegistration.WebApi.Services;

public interface IWeatherForecastService
{
    IEnumerable<WeatherForecast> Get();
}