using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.ConnectFour;

[NamedCommand("connectfour", Aliases = ["connect-four", "c4", "connect4"])]
public class CreateConnectFourCommand : Command
{
    private readonly IRoomsManager _roomsManager;
    private readonly IDependencyContainerService _dependencyContainerService;
    private readonly IConfigurationManager _configurationManager;

    public CreateConnectFourCommand(IRoomsManager roomsManager,
        IDependencyContainerService dependencyContainerService,
        IConfigurationManager configurationManager)
    {
        _roomsManager = roomsManager;
        _dependencyContainerService = dependencyContainerService;
        _configurationManager = configurationManager;
    }

    public override char RequiredRank => '+';

    public override Task Run(IContext context)
    {
        var room = _roomsManager.GetRoom(context.RoomId);
        if (room.Game is not null)
        {
            context.ReplyLocalizedMessage("c4_game_start_already_exist");
            return Task.CompletedTask;
        }

        var game = _dependencyContainerService.Resolve<ConnectFourGame>();
        game.Context = context;
        room.Game = game;
        context.ReplyLocalizedMessage("c4_game_start_announce", _configurationManager.Configuration.Trigger);
        return Task.CompletedTask;
    }
}