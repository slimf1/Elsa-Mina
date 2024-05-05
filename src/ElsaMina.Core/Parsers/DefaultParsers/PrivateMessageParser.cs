using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Parsers.DefaultParsers;

public abstract class PrivateMessageParser : MessageParser
{
    protected PrivateMessageParser(IContextFactory contextFactory) : base(contextFactory)
    {
    }

    protected override ContextType HandledContextType => ContextType.Pm;
}