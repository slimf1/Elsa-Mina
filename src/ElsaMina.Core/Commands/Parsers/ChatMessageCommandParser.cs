using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Commands.Parsers;

public sealed class ChatMessageCommandParser : ChatMessageParser
{
    private readonly IRoomsManager _roomsManager;
    private readonly IConfigurationManager _configurationManager;
    private readonly ICommandExecutor _commandExecutor;
    
    public ChatMessageCommandParser(IDependencyContainerService dependencyContainerService,
        IRoomsManager roomsManager,
        IConfigurationManager configurationManager,
        ICommandExecutor commandExecutor)
        : base(dependencyContainerService)
    {
        _roomsManager = roomsManager;
        _configurationManager = configurationManager;
        _commandExecutor = commandExecutor;
    }

    public override string Identifier => nameof(ChatMessageCommandParser);

    protected override async Task HandleChatMessage(IContext context)
    {
        if (context.RoomId == null || !_roomsManager.HasRoom(context.RoomId))
        {
            return;
        }
        if (_configurationManager.Configuration.RoomBlacklist.Contains(context.RoomId))
        {
            return;
        }

        if (context.Command == null)
        {
            return;
        }
        try
        {
            await _commandExecutor.TryExecuteCommand(context.Command, context);
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "Room Command execution crashed");
        }
    }
}