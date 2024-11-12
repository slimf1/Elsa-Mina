using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public sealed class ChatMessageCommandHandler : CommandMessageHandler
{
    public ChatMessageCommandHandler(IContextFactory contextFactory,
        IRoomsManager roomsManager,
        IConfigurationManager configurationManager,
        ICommandExecutor commandExecutor)
        : base(contextFactory, roomsManager, configurationManager, commandExecutor)
    {
    }

    protected override ContextType HandledContextType => ContextType.Room;
}