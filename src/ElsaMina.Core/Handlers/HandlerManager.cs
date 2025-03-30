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

    public void Initialize()
    {
        var handlers = _containerService.Resolve<IEnumerable<IHandler>>().ToList();
        foreach (var handler in handlers)
        {
            _handlers[handler.Identifier] = handler;
        }

        IsInitialized = true;
    }

    public async Task HandleMessageAsync(string[] parts, string roomId = null, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _handlers
                .Values
                .Where(handler => handler.IsEnabled)
                .Select(handler => handler.OnMessageReceivedAsync(parts, roomId, cancellationToken))
        );
    }
}