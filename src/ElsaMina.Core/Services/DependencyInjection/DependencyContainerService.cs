using Autofac;
using Autofac.Core;
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

    public IEnumerable<T> GetAllNamedRegistrations<T>() where T : class
    {
        if (_container is null)
        {
            return [];
        }
        var results = new List<T>();
        foreach (var registration in _container.ComponentRegistry.Registrations)
        {
            foreach (var keyedService in registration.Services.OfType<KeyedService>())
            {
                if (keyedService.ServiceType == typeof(T) && keyedService.ServiceKey is string name)
                {
                    results.Add(_container.ResolveNamed<T>(name));
                }
            }
        }

        return results;
    }
}