using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Parsers;

public class PrivateMessageCommandParser : CommandMessageParser
{
    public PrivateMessageCommandParser(IContextFactory contextFactory,
        IRoomsManager roomsManager,
        IConfigurationManager configurationManager,
        ICommandExecutor commandExecutor) : base(contextFactory, roomsManager, configurationManager, commandExecutor)
    {
    }

    public override string Identifier => nameof(PrivateMessageCommandParser);

    protected override ContextType HandledContextType => ContextType.Pm;
}