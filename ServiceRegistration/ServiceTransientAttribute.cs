using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceRegistration;

/// <summary>Transient service implementation</summary>
[AttributeUsage(validOn: AttributeTargets.Class)]
public sealed class ServiceTransientAttribute : ServiceAttribute
{
    public ServiceTransientAttribute(Type? serviceType = null) :
        base(ServiceLifetime.Transient, serviceType)
    {
    }
}