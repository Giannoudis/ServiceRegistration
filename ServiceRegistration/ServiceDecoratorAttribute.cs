using System;
using System.Reflection;

namespace ServiceRegistration;

/// <summary>Service implementation decorator</summary>
/// <remarks>Component must be a type with the <see cref="ServiceDecoratorAttribute"/> or <see cref="ServiceAttribute"/> </remarks>
[AttributeUsage(validOn: AttributeTargets.Class)]
public sealed class ServiceDecoratorAttribute : Attribute
{
    /// <summary>The component type</summary>
    public Type ImplementationType { get; set; }

    public ServiceDecoratorAttribute(Type implementationType)
    {
        ImplementationType = implementationType;

        // test mandatory attributes on the implementation
        if (!implementationType.IsDefined(typeof(ServiceDecoratorAttribute)) &&
            !implementationType.IsDefined(typeof(ServiceAttribute)))
        {
            throw new ServiceRegistrationException($"Service decorator implementation type {implementationType} has no service-implementation or service-decorator attribute");
        }
    }
}