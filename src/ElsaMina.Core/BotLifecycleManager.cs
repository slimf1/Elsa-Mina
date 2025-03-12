using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Core;

public class BotLifecycleManager : IBotLifecycleManager
{
    private readonly IDependencyContainerService _dependencyContainerService;
    private IEnumerable<IBotLifecycleHandler> _handlers;

    public BotLifecycleManager(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }

    private IEnumerable<IBotLifecycleHandler> Handlers =>
        _handlers ??= _dependencyContainerService.Resolve<IEnumerable<IBotLifecycleHandler>>();

    public void OnStart()
    {
        foreach (var handler in Handlers)
        {
            handler.OnStart();
        }
    }

    public void OnReconnect()
    {
        foreach (var handler in Handlers)
        {
            handler.OnReconnect();
        }
    }

    public void OnDisconnect()
    {
        foreach (var handler in Handlers)
        {
            handler.OnDisconnect();
        }
    }
}