using System;

namespace ServiceRegistration.Service;

/// <summary>Decorator reflection data</summary>
/// <param name="DecoratorType">The decorator type</param>
/// <param name="ComponentType">The component type</param>
/// <param name="ServiceType">The service type</param>
public sealed record DecoratorReflectionData(Type DecoratorType, Type ComponentType, Type ServiceType);