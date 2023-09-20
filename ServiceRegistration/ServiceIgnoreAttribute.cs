using System;

namespace ServiceRegistration;

/// <summary>Ignore from service registration</summary>
[AttributeUsage(validOn: AttributeTargets.Interface | AttributeTargets.Class)]
public sealed class ServiceIgnoreAttribute : Attribute
{
}