using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceRegistration;

/// <summary>Base class for service registration attribute</summary>
public abstract class ServiceAttribute : Attribute
{
    private Type? ServiceType { get; }
    public ServiceLifetime Lifetime { get; }

    protected ServiceAttribute(ServiceLifetime lifetime, Type? serviceType = null)
    {
        ServiceType = serviceType;
        Lifetime = lifetime;

        // test service is an interface
        if (ServiceType != null && !ServiceType.IsInterface)
        {
            throw new ServiceRegistrationException($"Service {ServiceType} type must be an interface");
        }
    }

    public Type? GetServiceType(Type implementationType)
    {
        if (ServiceType != null)
        {
            return ServiceType;
        }
        var interfaces = implementationType.GetInterfaces().ToList();
        if (interfaces.Count > 1)
        {
            throw new ServiceRegistrationException($"Service implementation type {implementationType} with multiple interfaces {string.Join(", ", interfaces)}");
        }
        var serviceType = interfaces.FirstOrDefault();
        return serviceType;
    }
}