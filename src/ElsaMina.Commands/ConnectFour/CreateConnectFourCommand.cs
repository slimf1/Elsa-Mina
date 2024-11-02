using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.ConnectFour;

[NamedCommand("connectfour", Aliases = ["connect-four", "c4", "connect4"])]
public class CreateConnectFourCommand : Command
{
    private readonly IRoomsManager _roomsManager;
    private readonly IDependencyContainerService _dependencyContainerService;

    public CreateConnectFourCommand(IRoomsManager roomsManager,
        IDependencyContainerService dependencyContainerService)
    {
        _roomsManager = roomsManager;
        _dependencyContainerService = dependencyContainerService;
    }

    public override char RequiredRank => '+';

    public override Task Run(IContext context)
    {
        var room = _roomsManager.GetRoom(context.RoomId);
        if (room.Game is not null)
        {
            context.Reply("A game is already ongoing."); // todo: i18n
            return Task.CompletedTask;
        }

        var game = _dependencyContainerService.Resolve<ConnectFourGame>();
        game.Context = context;
        room.Game = game;
        context.Reply("A connect four game has been created. Use &c4j to join.");
        return Task.CompletedTask;
    }
}