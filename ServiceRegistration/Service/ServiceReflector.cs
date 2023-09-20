using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServiceRegistration.Service;

/// <summary>Service reflector</summary>
public sealed class ServiceReflector
{
    private const string SystemNamespace = $"{nameof(System)}.";

    /// <summary>The reflection assemblies</summary>
    public IList<Assembly> Assemblies { get; }

    public ServiceReflector(IList<Assembly>? assemblies = null)
    {
        Assemblies = assemblies ?? GetDomainAssemblies();
    }

    /// <summary>Get all domain assemblies, excluding the system assemblies</summary>
    private static List<Assembly> GetDomainAssemblies() =>
        AppDomain.CurrentDomain.GetAssemblies().
            Where(x => !x.GetName().FullName.StartsWith(SystemNamespace)).ToList();


    /// <summary>Reflect assembly services</summary>
    /// <param name="serviceQuery">The service query</param>
    /// <returns>The service reflection</returns>
    public ReflectionData Reflect(RegistrationOptions? serviceQuery)
    {
        var result = new ReflectionData();

        // assemblies
        foreach (var curAssembly in Assemblies)
        {
            // assembly filter
            if (serviceQuery?.AssemblyFilter != null && serviceQuery.AssemblyFilter(curAssembly) == false)
            {
                continue;
            }

            // types
            foreach (var curType in curAssembly.GetTypes())
            {
                ReflectType(curType, serviceQuery, result);
            }
        }

        return result;
    }

    /// <summary>Reflect a potential service type</summary>
    /// <param name="type">The type to reflect</param>
    /// <param name="serviceQuery">The service query</param>
    /// <param name="reflection">The reflection data</param>
    private static void ReflectType(Type type, RegistrationOptions? serviceQuery, ReflectionData reflection)
    {
        var customAttributes = type.GetCustomAttributes().ToList();

        // ignored service or implementation
        if (customAttributes.Any(x => x is ServiceIgnoreAttribute))
        {
            return;
        }

        // type filter
        if (!FilterType(type, serviceQuery))
        {
            return;
        }

        // implementation type
        if (customAttributes.FirstOrDefault(x => x is ServiceAttribute)
            is ServiceAttribute implementationAttribute)
        {
            reflection.ImplementationTypes.Add(type, implementationAttribute);
            return;
        }

        // decorator type
        if (customAttributes.FirstOrDefault(x => x is ServiceDecoratorAttribute)
            is ServiceDecoratorAttribute decoratorAttribute)
        {
            var serviceType = decoratorAttribute.ImplementationType.GetDecoratedImplementation();
            if (serviceType == null)
            {
                throw new ServiceRegistrationException(
                    $"Missing service implementation in implementation type {decoratorAttribute.ImplementationType}");
            }

            if (type.GetInterface(serviceType.Name) == null)
            {
                throw new ServiceRegistrationException(
                    $"Missing service {serviceType} on decorator: {type}");
            }

            reflection.DecoratorTypes.Add(new(type, decoratorAttribute.ImplementationType, serviceType));
        }
    }

    /// <summary>Reflect the type by interfaces</summary>
    /// <param name="type">The type to reflect</param>
    /// <param name="serviceQuery">The service query</param>
    /// <returns>True if service is registered</returns>
    private static bool FilterType(Type type, RegistrationOptions? serviceQuery)
    {
        // custom type filter
        return serviceQuery?.TypeFilter == null || serviceQuery.TypeFilter(type);
    }
}