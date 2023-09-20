using System;
using System.Collections.Generic;

namespace ServiceRegistration.Service;

/// <summary>Service reflection data</summary>
public sealed class ReflectionData
{
    /// <summary>The implementation types</summary>
    public Dictionary<Type, ServiceAttribute> ImplementationTypes { get; } = new();

    /// <summary>The decorator types</summary>
    public List<DecoratorReflectionData> DecoratorTypes { get; } = new();

    /// <summary>The service registrations</summary>
    public List<ServiceRegistration> Registrations { get; } = new();
}