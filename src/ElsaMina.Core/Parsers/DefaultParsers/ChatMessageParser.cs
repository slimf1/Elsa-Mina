using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Parsers.DefaultParsers;

public abstract class ChatMessageParser : MessageParser
{
    protected ChatMessageParser(IContextFactory contextFactory) : base(contextFactory)
    {
    }

    protected override ContextType HandledContextType => ContextType.Room;
}