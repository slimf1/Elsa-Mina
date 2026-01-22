using Autofac;
using Autofac.Core;

namespace ElsaMina.Core.Services.DependencyInjection;

public class DependencyContainerService : IDependencyContainerService
{
    public static IDependencyContainerService Current { get; set; }

    private IContainer _container;

    public void SetContainer(IContainer container)
    {
        _container = container;
    }

    public T Resolve<T>() where T : notnull
    {
        return _container == null ? default : _container.Resolve<T>();
    }

    public T ResolveNamed<T>(string name)
    {
        return _container == null ? default : _container.ResolveNamed<T>(name);
    }

    public bool IsRegisteredWithName<T>(string name)
    {
        return _container?.IsRegisteredWithName<T>(name) ?? false;
    }

    public IEnumerable<T> GetAllNamedRegistrations<T>() where T : class
    {
        if (_container is null)
        {
            return [];
        }

        return _container.ComponentRegistry.Registrations
            .SelectMany(registration => registration.Services.OfType<KeyedService>()
                .Where(service => service.ServiceType == typeof(T) && service.ServiceKey is string)
                .Select(service => service.ServiceKey as string))
            .Select(name => _container.ResolveNamed<T>(name))
            .ToList();
    }
}