using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Commands.Parsers;

public abstract class ChatMessageParser : MessageParser
{
    protected ChatMessageParser(IContextFactory contextFactory) : base(contextFactory)
    {
    }

    protected override ContextType HandledContextType => ContextType.Room;
}