using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Commands.Parsers;

public abstract class PrivateMessageParser : MessageParser
{
    protected PrivateMessageParser(IContextFactory contextFactory) : base(contextFactory)
    {
    }

    protected override ContextType HandledContextType => ContextType.Pm;
}