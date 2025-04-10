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
                .Select(handler => TryHandleMessageAsync(parts, roomId, cancellationToken, handler))
        );
    }

    private static Task TryHandleMessageAsync(string[] parts, string roomId, CancellationToken cancellationToken, IHandler handler)
    {
        try
        {
            return handler.HandleReceivedMessageAsync(parts, roomId, cancellationToken);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Error while handling message in handler {Handler}", handler.Identifier);
            throw;
        }
    }
}