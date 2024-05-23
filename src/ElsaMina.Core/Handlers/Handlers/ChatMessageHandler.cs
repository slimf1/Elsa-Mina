using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Handlers.Handlers;

public abstract class ChatMessageHandler : MessageHandler
{
    protected ChatMessageHandler(IContextFactory contextFactory) : base(contextFactory)
    {
    }

    protected override ContextType HandledContextType => ContextType.Room;
}