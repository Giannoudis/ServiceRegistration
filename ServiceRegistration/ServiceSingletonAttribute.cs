using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceRegistration;

/// <summary>Singleton service implementation</summary>
[AttributeUsage(validOn: AttributeTargets.Class)]
public sealed class ServiceSingletonAttribute : ServiceAttribute
{
    public ServiceSingletonAttribute(Type? serviceType = null) :
        base(ServiceLifetime.Singleton, serviceType)
    {
    }
}