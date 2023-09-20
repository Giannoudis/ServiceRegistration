namespace ServiceRegistration;

/// <summary>Service implementation decorator interface</summary>
public interface IServiceDecorator<out T> where T : class
{
    /// <summary>The service implementation</summary>
    public T Implementation { get; }
}