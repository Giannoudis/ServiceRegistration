using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServiceRegistration.Service;

/// <summary>Service collector</summary>
public class ServiceCollector
{
    /// <summary>The service reflection</summary>
    public ReflectionData Reflection { get; }

    public ServiceCollector(ReflectionData reflection)
    {
        Reflection = reflection ?? throw new ArgumentNullException(nameof(reflection));
    }

    /// <summary>Collect services</summary>
    /// <param name="serviceQuery">The service query</param>
    /// <returns>The service registrations</returns>
    public List<ServiceRegistration> Collect(RegistrationOptions? serviceQuery = null)
    {
        // register implementations
        RegisterImplementations();
        // register decorators
        RegisterDecorators();

        // empty reflection
        if (Reflection.Registrations.Count <= 1)
        {
            return Reflection.Registrations;
        }

        // conflicts
        var services = ResolveServiceRegistrations(serviceQuery, Reflection.Registrations);
        // mappings
        MapServices(serviceQuery, services);

        return services;
    }

    #region Register

    /// <summary>Register all implementations</summary>
    private void RegisterImplementations()
    {
        foreach (var implementationType in Reflection.ImplementationTypes)
        {
            var serviceType = implementationType.Value.GetServiceType(implementationType.Key);
            if (serviceType == null)
            {
                throw new ServiceRegistrationException(
                    $"Missing service type in implementation type {implementationType.Key}");
            }

            var registration = new ServiceRegistration(serviceType, implementationType.Key,
                implementationType.Value.Lifetime);
            if (!Reflection.Registrations.Contains(registration))
            {
                Reflection.Registrations.Add(registration);
            }
        }
    }

    #endregion

    #region Decorator

    /// <summary>Register all decorators</summary>
    private void RegisterDecorators()
    {
        // group decorators by target service
        var serviceGroups = Reflection.DecoratorTypes
            .GroupBy(key => key.ServiceType, value => value);

        foreach (var serviceGroup in serviceGroups)
        {
            var decorators = GetOrderedDecorators(serviceGroup.ToList());
            foreach (var decorator in decorators)
            {
                RegisterDecorator(decorator, Reflection.Registrations, serviceGroup.Key, decorator == decorators.Last());
            }
        }
    }

    /// <summary>Register decorator</summary>
    /// <param name="decorator">The reflection decorator</param>
    /// <param name="registrations">The service registrations</param>
    /// <param name="serviceType">The service type</param>
    /// <param name="last">The last service decorator</param>
    private static void RegisterDecorator(DecoratorReflectionData decorator, List<ServiceRegistration> registrations,
        Type serviceType, bool last)
    {
        var implementation = GetDecoratedImplementation(decorator.DecoratorType);
        if (implementation == null)
        {
            throw new ServiceRegistrationException(
                $"Missing service implementation {decorator.ComponentType} on decorator {decorator.DecoratorType}");
        }

        // replacing implementation registration
        var registration = new ServiceRegistration(
            DecoratorType: decorator.DecoratorType,
            ImplementationType: decorator.ComponentType,
            ServiceType: serviceType,
            Lifetime: implementation.Item2.Lifetime);
        var index = registrations.IndexOf(registration);
        if (index < 0)
        {
            registrations.Add(registration);
            index = registrations.IndexOf(registration);
        }

        // decorator registration
        var decoratorServiceType = typeof(IServiceDecorator<>).MakeGenericType(decorator.ComponentType);
        var implementationType = typeof(ServiceDecorator<>).MakeGenericType(decorator.ComponentType);
        var decoratedRegistration = new ServiceRegistration(
            ServiceType: decoratorServiceType,
            ImplementationType: implementationType,
            Lifetime: registration.Lifetime);
        if (!registrations.Contains(decoratedRegistration))
        {
            index++;
            registrations.Insert(index, decoratedRegistration);
        }

        // service registration
        if (last)
        {
            var existing = registrations.FirstOrDefault(x => x.ServiceType == implementation.Item2.GetServiceType(implementation.Item1) &&
                                                             x.ImplementationType == implementation.Item1 &&
                                                             x.Lifetime == implementation.Item2.Lifetime);
            if (existing == null)
            {
                throw new ServiceRegistrationException(
                    $"Missing decorator implementation {decorator.ComponentType} on decorator {decorator.DecoratorType}");
            }

            var decoratorRegistration = new ServiceRegistration(
                ServiceType: registration.ServiceType,
                ImplementationType: decorator.DecoratorType,
                Lifetime: registration.Lifetime);
            index = registrations.IndexOf(existing);
            registrations.Insert(index, decoratorRegistration);
            registrations.Remove(existing);
        }
    }

    /// <summary>Order the decorators for registering</summary>
    /// <param name="decorators">The decorators to order</param>
    /// <returns>Ordered decorators</returns>
    private static List<DecoratorReflectionData> GetOrderedDecorators(IList<DecoratorReflectionData> decorators)
    {
        // test for multiple decorator registrations
        var count = decorators.Count(x => decorators.All(y => x.DecoratorType != y.ComponentType));
        if (count > 1)
        {
            throw new ServiceRegistrationException(
                $"Multiple decorator registrations: {string.Join(", ", decorators)}");
        }

        // sort by type
        var orderedDecorators = new List<DecoratorReflectionData>(decorators);
        for (var i = 0; i < orderedDecorators.Count; i++)
        {
            for (var y = 0; y < orderedDecorators.Count; y++)
            {
                if (y == i)
                {
                    continue;
                }
                // decorated decorator
                // ensure the decorated type is registered before the decorator
                if (orderedDecorators[i].DecoratorType == orderedDecorators[y].ComponentType)
                {
                    // swap
                    (orderedDecorators[i], orderedDecorators[y]) = (orderedDecorators[y], orderedDecorators[i]);
                }
            }
        }
        return orderedDecorators;
    }

    /// <summary>Get decorated implementation</summary>
    /// <param name="decoratorType">THe decorator type</param>
    /// <returns>Tuple with the decorator type and the implementation attribute</returns>
    private static Tuple<Type, ServiceAttribute>? GetDecoratedImplementation(Type decoratorType)
    {
        // sub decorator
        if (decoratorType.GetCustomAttributes().FirstOrDefault(x => x is ServiceDecoratorAttribute)
            is ServiceDecoratorAttribute decoratorAttribute)
        {
            //     return new(decoratorAttribute.ImplementationType, decoratorAttribute.ImplementationType, decoratorAttribute.Lifetime, true);
            var implementation = GetDecoratedImplementation(decoratorAttribute.ImplementationType);
            if (implementation != null)
            {
                return implementation;
            }
        }

        // target service
        if (decoratorType.GetCustomAttributes().FirstOrDefault(x => x is ServiceAttribute)
            is ServiceAttribute implementationAttribute)
        {
            return new(decoratorType, implementationAttribute);
        }

        return null;
    }

    #endregion

    #region Conflict and Map

    /// <summary>Resolve service registration conflicts</summary>
    /// <param name="serviceQuery">THe service query</param>
    /// <param name="registrations">THe service registration to check</param>
    /// <returns>The resolve registration services</returns>
    private static List<ServiceRegistration> ResolveServiceRegistrations(RegistrationOptions? serviceQuery, List<ServiceRegistration> registrations)
    {
        // ignore decorated types
        var cleanRegistrations = new List<ServiceRegistration>(registrations.Where(x => x.DecoratorType != null));

        var serviceGroups = registrations
            .Where(x => x.DecoratorType == null)
            .GroupBy(key => new { key.ServiceType, key.Lifetime }, value => value);
        foreach (var serviceGroup in serviceGroups)
        {
            if (serviceGroup.Count() == 1)
            {
                cleanRegistrations.Add(serviceGroup.First());
                continue;
            }

            if (!serviceGroup.Any())
            {
                continue;
            }

            // multiple service registration
            ServiceRegistration? resolved = null;
            if (serviceQuery?.ResolveRegistration != null)
            {
                var conflicts = serviceGroup.ToList();
                resolved = serviceQuery.ResolveRegistration(
                    serviceGroup.Key.ServiceType,
                    serviceGroup.Key.Lifetime,
                    conflicts);
                // resolved must be from the conflicts
                if (!conflicts.Contains(resolved))
                {
                    resolved = null;
                }
            }

            if (resolved == null)
            {
                throw new ServiceRegistrationException(
                    $"Multiple service registrations: {string.Join(", ", serviceGroup)}");
            }

            // resolved
            cleanRegistrations.Add(resolved);
        }

        return cleanRegistrations;
    }

    /// <summary>Map registration services</summary>
    /// <param name="serviceQuery">THe service query</param>
    /// <param name="registrations">THe service registration to check</param>
    private static void MapServices(RegistrationOptions? serviceQuery, List<ServiceRegistration> registrations)
    {
        if (serviceQuery?.MapRegistration == null)
        {
            return;
        }

        var maps = new List<Tuple<ServiceRegistration, ServiceRegistration>>();
        foreach (var registration in registrations)
        {
            var implementationType = serviceQuery.MapRegistration(registration);
            if (implementationType == registration.ImplementationType)
            {
                continue;
            }

            // old and new registration
            maps.Add(new Tuple<ServiceRegistration, ServiceRegistration>(
                registration,
                new ServiceRegistration(
                    registration.ServiceType,
                    implementationType,
                    registration.Lifetime)));
        }

        // apply mappings
        foreach (var map in maps)
        {
            var index = registrations.IndexOf(map.Item1);
            registrations.Insert(index, map.Item2);
            registrations.Remove(map.Item1);
        }
    }

    #endregion

}