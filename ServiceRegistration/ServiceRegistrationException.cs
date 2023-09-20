using System;
using System.Runtime.Serialization;

namespace ServiceRegistration;

/// <summary>Service registration exception</summary>
public class ServiceRegistrationException : Exception
{
    /// <inheritdoc/>
    public ServiceRegistrationException()
    {
    }

    /// <inheritdoc/>
    public ServiceRegistrationException(string message) :
        base(message)
    {
    }

    /// <inheritdoc/>
    public ServiceRegistrationException(string message, Exception innerException) :
        base(message, innerException)
    {
    }

    /// <inheritdoc/>
    protected ServiceRegistrationException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}