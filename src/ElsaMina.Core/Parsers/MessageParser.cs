using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Parsers;

public abstract class MessageParser : Parser
{
    private readonly IContextFactory _contextFactory;

    protected MessageParser(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    protected abstract ContextType HandledContextType { get; }

    protected sealed override async Task Execute(string[] parts, string roomId = null)
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