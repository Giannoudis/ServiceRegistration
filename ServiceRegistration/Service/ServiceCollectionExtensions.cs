using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceRegistration.Service;

/// <summary>Extensions for <see cref="IServiceCollection"/></summary>
public static class ServiceCollectionExtensions
{

    #region Services

    /// <summary>Register services</summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="serviceQuery">The service query</param>
    /// <param name="assemblies">The assemblies to reflect (default: current domain assemblies)</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterServices(this IServiceCollection serviceCollection,
        RegistrationOptions? serviceQuery = null, IList<Assembly>? assemblies = null)
    {
        // reflection
        var reflection = new ServiceReflector(assemblies).Reflect(serviceQuery);

        // service collection
        var services = new ServiceCollector(reflection).Collect(serviceQuery);

        // dependency injection
        foreach (var service in services)
        {
            if (service.DecoratorType != null)
            {
                // decorated dependencies
                switch (service.Lifetime)
                {
                    case ServiceLifetime.Transient:
                        serviceCollection.AddTransient(service.ImplementationType);
                        break;
                    case ServiceLifetime.Scoped:
                        serviceCollection.AddScoped(service.ImplementationType);
                        break;
                    case ServiceLifetime.Singleton:
                        serviceCollection.AddSingleton(service.ImplementationType);
                        break;
                }
            }
            else
            {
                // service dependencies
                switch (service.Lifetime)
                {
                    case ServiceLifetime.Transient:
                        serviceCollection.AddTransient(service.ServiceType, service.ImplementationType);
                        break;
                    case ServiceLifetime.Scoped:
                        serviceCollection.AddScoped(service.ServiceType, service.ImplementationType);
                        break;
                    case ServiceLifetime.Singleton:
                        serviceCollection.AddSingleton(service.ServiceType, service.ImplementationType);
                        break;
                }
            }
        }

        return serviceCollection;
    }

    #endregion

    #region Decorate

    /// <summary>Decorate a transient type</summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection DecorateService<TService, TDecorator, TComponent>(this IServiceCollection serviceCollection) =>
        serviceCollection.DecorateService(typeof(TService), typeof(TDecorator), typeof(TComponent));

    /// <summary>Decorate a type</summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="serviceType">The service type</param>
    /// <param name="decoratorType">The decorator type</param>
    /// <param name="componentType">The component type</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection DecorateService(this IServiceCollection serviceCollection,
        Type serviceType, Type decoratorType, Type componentType)
    {
        // service
        var service = serviceCollection.FirstOrDefault(x =>
            x.ServiceType == serviceType);
        if (service == null)
        {
            throw new ServiceRegistrationException($"Unknown service type {serviceType}");
        }

        // decorated type registration
        switch (service.Lifetime)
        {
            case ServiceLifetime.Transient:
                serviceCollection.AddTransient(componentType);
                break;
            case ServiceLifetime.Scoped:
                serviceCollection.AddScoped(componentType);
                break;
            case ServiceLifetime.Singleton:
                serviceCollection.AddSingleton(componentType);
                break;
        }

        // decorator registration
        var decoratorService = typeof(IServiceDecorator<>).MakeGenericType(componentType);
        var decoratorImplementation = typeof(ServiceDecorator<>).MakeGenericType(componentType);
        switch (service.Lifetime)
        {
            case ServiceLifetime.Transient:
                serviceCollection.AddTransient(decoratorService, decoratorImplementation);
                break;
            case ServiceLifetime.Scoped:
                serviceCollection.AddScoped(decoratorService, decoratorImplementation);
                break;
            case ServiceLifetime.Singleton:
                serviceCollection.AddSingleton(decoratorService, decoratorImplementation);
                break;
        }

        // service registration
        // remove existing service
        serviceCollection.Remove(service);
        switch (service.Lifetime)
        {
            case ServiceLifetime.Transient:
                serviceCollection.AddTransient(serviceType, decoratorType);
                break;
            case ServiceLifetime.Scoped:
                serviceCollection.AddScoped(serviceType, decoratorType);
                break;
            case ServiceLifetime.Singleton:
                serviceCollection.AddSingleton(serviceType, decoratorType);
                break;
        }
        return serviceCollection;
    }

    #endregion

}