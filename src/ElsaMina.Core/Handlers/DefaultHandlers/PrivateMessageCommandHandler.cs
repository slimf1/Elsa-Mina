using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public sealed class PrivateMessageCommandHandler : CommandMessageHandler
{
    public PrivateMessageCommandHandler(IContextFactory contextFactory,
        IRoomsManager roomsManager,
        IConfigurationManager configurationManager,
        ICommandExecutor commandExecutor) : base(contextFactory, roomsManager, configurationManager, commandExecutor)
    {
    }

    public override string Identifier => nameof(PrivateMessageCommandHandler);

    protected override ContextType HandledContextType => ContextType.Pm;
}