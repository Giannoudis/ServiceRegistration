using System;
using System.Linq;
using System.Reflection;

namespace ServiceRegistration;

internal static class TypeExtensions
{
    /// <summary>Get the implementation type of the decorated service</summary>
    /// <param name="type">The type to reflect</param>
    /// <returns>The service type, null on missing service type</returns>
    internal static Type? GetDecoratedImplementation(this Type type)
    {
        // sub decorator
        if (type.GetCustomAttributes().FirstOrDefault(x => x is ServiceDecoratorAttribute)
            is ServiceDecoratorAttribute decoratorAttribute)
        {
            return GetDecoratedImplementation(decoratorAttribute.ImplementationType);
        }

        // target service
        if (type.GetCustomAttributes().FirstOrDefault(x => x is ServiceAttribute)
            is ServiceAttribute implementationAttribute)
        {
            return implementationAttribute.GetServiceType(type);
        }

        return null;
    }

}