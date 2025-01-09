using Autofac;
using ElsaMina.Core.Commands;

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

    public IEnumerable<T> GetAllRegistrations<T>() where T : class
    {
        if (_container is null)
        {
            return [];
        }

        return _container.ComponentRegistry.Registrations
            .Where(r => typeof(T).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Select(type => _container.Resolve(type) as T);
    }
}