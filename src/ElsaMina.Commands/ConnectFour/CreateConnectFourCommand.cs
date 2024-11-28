using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
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

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task Run(IContext context)
    {
        var room = _roomsManager.GetRoom(context.RoomId);
        if (room.Game is not null)
        {
            context.ReplyLocalizedMessage("c4_game_start_already_exist");
            return;
        }

        var game = _dependencyContainerService.Resolve<ConnectFourGame>();
        game.Context = context;
        room.Game = game;
        await game.DisplayAnnounce();
    }
}