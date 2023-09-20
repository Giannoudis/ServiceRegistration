using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceRegistration.Service;

/// <summary>Service registration</summary>
/// <param name="ServiceType">The service type</param>
/// <param name="ImplementationType">The implementation type</param>
/// <param name="Lifetime">The service lifetime</param>
/// <param name="DecoratorType">The decorator type</param>
public record ServiceRegistration(Type ServiceType, Type ImplementationType, ServiceLifetime Lifetime, Type? DecoratorType = null)
{
    /// <summary>The service type</summary>
    public Type ServiceType { get; init; } = ServiceType ?? throw new ArgumentNullException(nameof(ServiceType));

    /// <summary>The implementation type</summary>
    public Type ImplementationType { get; init; } = ImplementationType ?? throw new ArgumentNullException(nameof(ImplementationType));

    /// <summary>The service lifetime</summary>
    public ServiceLifetime Lifetime { get; init; } = Lifetime;

    /// <summary>The decorator type</summary>
    public Type? DecoratorType { get; init; } = DecoratorType;

    public override string ToString() =>
        $"{ServiceType.Name} > {ImplementationType.Name} ({Lifetime}){(DecoratorType != null ? $" [{DecoratorType.Name}]" : string.Empty)}";
}