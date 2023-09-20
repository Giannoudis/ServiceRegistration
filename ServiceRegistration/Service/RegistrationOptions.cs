using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceRegistration.Service;

/// <summary>Registration options</summary>
public sealed class RegistrationOptions
{
    /// <summary>The assembly function</summary>
    /// <remarks>Function parameter: assembly</remarks>
    /// <remarks>Function result: false to ignore the assembly</remarks>
    public Func<Assembly, bool>? AssemblyFilter { get; set; }

    /// <summary>The type function</summary>
    /// <remarks>Function parameter: current type</remarks>
    /// <remarks>Function result: service registration</remarks>
    public Func<Type, bool>? TypeFilter { get; set; }

    /// <summary>Resolve registration function</summary>
    /// <remarks>Function parameter: service type, injection mode, conflicting registrations</remarks>
    /// <remarks>Function result: service registration (mandatory)</remarks>
    public Func<Type, ServiceLifetime, IList<ServiceRegistration>, ServiceRegistration>? ResolveRegistration { get; set; }

    /// <summary>Service map</summary>
    /// <remarks>Function parameter: service type, implementation injection mode</remarks>
    /// <remarks>Function result: implementation type</remarks>
    public Func<ServiceRegistration, Type>? MapRegistration { get; set; }

    /// <summary>Add report services</summary>
    /// <param name="assemblyFilter">The assembly function</param>
    /// <param name="typeFilter">The type function</param>
    /// <param name="resolveRegistration">The resolve function</param>
    /// <param name="mapRegistration">The service map function</param>
    public RegistrationOptions(
        Func<Assembly, bool>? assemblyFilter = null,
        Func<Type, bool>? typeFilter = null,
        Func<Type, ServiceLifetime, IList<ServiceRegistration>, ServiceRegistration>? resolveRegistration = null,
        Func<ServiceRegistration, Type>? mapRegistration = null)
    {
        AssemblyFilter = assemblyFilter;
        TypeFilter = typeFilter;
        ResolveRegistration = resolveRegistration;
        MapRegistration = mapRegistration;
    }
}