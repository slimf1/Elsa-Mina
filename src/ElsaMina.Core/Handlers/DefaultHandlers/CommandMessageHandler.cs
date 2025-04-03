using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public abstract class CommandMessageHandler : MessageHandler
{
    private readonly IRoomsManager _roomsManager;
    private readonly IConfiguration _configuration;
    private readonly ICommandExecutor _commandExecutor;
    
    protected CommandMessageHandler(IContextFactory contextFactory,
        IRoomsManager roomsManager,
        IConfiguration configuration,
        ICommandExecutor commandExecutor) : base(contextFactory)
    {
        _roomsManager = roomsManager;
        _configuration = configuration;
        _commandExecutor = commandExecutor;
    }

    public override async Task HandleMessageAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (context.RoomId == null || !_roomsManager.HasRoom(context.RoomId))
        {
            return;
        }
        if (_configuration.RoomBlacklist.Contains(context.RoomId))
        {
            return;
        }

        if (context.Command == null)
        {
            return;
        }
        try
        {
            await _commandExecutor.TryExecuteCommandAsync(context.Command, context, cancellationToken);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Command execution crashed with context : {0}", context);
            context.HandleError(exception);
        }
    }
}