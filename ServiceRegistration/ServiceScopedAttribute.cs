using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceRegistration;

/// <summary>Scoped service implementation</summary>
[AttributeUsage(validOn: AttributeTargets.Class)]
public sealed class ServiceScopedAttribute : ServiceAttribute
{
    public ServiceScopedAttribute(Type? serviceType = null) :
        base(ServiceLifetime.Scoped, serviceType)
    {
    }
}