using ServiceRegistration.Service;
using ServiceRegistration.WebApi.Services;

namespace ServiceRegistration.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // service registration
        builder.Services.RegisterServices(new()
        {
            //AssemblyFilter = assembly => true,
            //TypeFilter = type => type.GetInterfaces().Any(x =>
            //    x.IsGenericType &&
            //    x.GetGenericTypeDefinition() == typeof(IRepository<>))
            //ResolveRegistration = (type, lifetime, conflicts) => conflicts.First(),
            //MapRegistration = registration => registration.ImplementationType
        });

        // service decoration by code
        builder.Services.DecorateService<IWeatherForecastService,
            Decorator3WeatherForecastService,
            Decorator2WeatherForecastService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}