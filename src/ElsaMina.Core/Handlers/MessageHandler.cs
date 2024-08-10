using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Handlers;

public abstract class MessageHandler : Handler
{
    private readonly IContextFactory _contextFactory;

    protected MessageHandler(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    protected abstract ContextType HandledContextType { get; }

    protected sealed override async Task HandleReceivedMessage(string[] parts, string roomId = null)
    {
        var context = _contextFactory.TryBuildContextFromReceivedMessage(parts, roomId);
        if (context == null || HandledContextType != context.Type)
        {
            return;
        }
        
        await HandleMessage(context);
    }

    protected abstract Task HandleMessage(IContext context);
}