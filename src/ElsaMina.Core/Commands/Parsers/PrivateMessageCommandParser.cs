using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;
using Serilog;

namespace ElsaMina.Core.Commands.Parsers;

public class PrivateMessageCommandParser : PrivateMessageParser
{
    private readonly ILogger _logger;
    private readonly IRoomsManager _roomsManager;
    private readonly IConfigurationManager _configurationManager;
    private readonly ICommandExecutor _commandExecutor;

    public PrivateMessageCommandParser(IDependencyContainerService dependencyContainerService,
        ILogger logger,
        IRoomsManager roomsManager,
        IConfigurationManager configurationManager,
        ICommandExecutor commandExecutor) : base(dependencyContainerService)
    {
        _logger = logger;
        _roomsManager = roomsManager;
        _configurationManager = configurationManager;
        _commandExecutor = commandExecutor;
    }

    protected override async Task HandlePrivateMessage(IContext context)
    {
        if (context.RoomId == null || !_roomsManager.HasRoom(context.RoomId))
        {
            return;
        }

        if (_configurationManager.Configuration.RoomBlacklist.Contains(context.RoomId))
        {
            return;
        }

        var (target, command) = Parsing.ParseMessage(context.Message,
            _configurationManager.Configuration.Trigger);
        if (target == null || command == null)
        {
            return;
        }

        try
        {
            await _commandExecutor.TryExecuteCommand(command, context);
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "Room Command execution crashed");
        }
    }
}