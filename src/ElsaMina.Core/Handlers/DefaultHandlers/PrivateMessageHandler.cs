using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public abstract class PrivateMessageHandler : MessageHandler
{
    protected PrivateMessageHandler(IContextFactory contextFactory) : base(contextFactory)
    {
    }

    protected override ContextType HandledContextType => ContextType.Pm;
}