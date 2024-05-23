using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Handlers.Handlers;

public abstract class CommandMessageHandler : MessageHandler
{
    private readonly IRoomsManager _roomsManager;
    private readonly IConfigurationManager _configurationManager;
    private readonly ICommandExecutor _commandExecutor;
    
    protected CommandMessageHandler(IContextFactory contextFactory,
        IRoomsManager roomsManager,
        IConfigurationManager configurationManager,
        ICommandExecutor commandExecutor) : base(contextFactory)
    {
        _roomsManager = roomsManager;
        _configurationManager = configurationManager;
        _commandExecutor = commandExecutor;
    }

    protected override async Task HandleMessage(IContext context)
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
            Logger.Current.Error(exception, "Command execution crashed with context : {0}", context);
        }
    }
}