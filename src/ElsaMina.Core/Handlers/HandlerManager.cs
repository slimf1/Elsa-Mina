using System.Collections.Concurrent;
using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Core.Handlers;

public class HandlerManager : IHandlerManager
{
    private readonly IDependencyContainerService _containerService;

    private readonly ConcurrentDictionary<string, IHandler> _handlers = [];

    public HandlerManager(IDependencyContainerService containerService)
    {
        _containerService = containerService;
    }

    public bool IsInitialized { get; private set; }

    public async Task Initialize()
    {
        var handlers = _containerService.Resolve<IEnumerable<IHandler>>().ToList();
        foreach (var handler in handlers)
        {
            _handlers[handler.Identifier] = handler;
        }

        await Task.WhenAll(handlers.Select(handler => handler.OnInitialize()));
        IsInitialized = true;
    }

    public async Task HandleMessage(string[] parts, string roomId = null)
    {
        await Task.WhenAll(
            _handlers
                .Values
                .Where(handler => handler.IsEnabled)
                .Select(handler => handler.Invoke(parts, roomId))
        );
    }
}