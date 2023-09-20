namespace ServiceRegistration;

/// <summary>Service implementation decorator</summary>
public class ServiceDecorator<T> : IServiceDecorator<T> where T : class
{
    /// <inheritdoc />
    public T Implementation { get; }

    public ServiceDecorator(T implementation)
    {
        Implementation = implementation;
    }
}