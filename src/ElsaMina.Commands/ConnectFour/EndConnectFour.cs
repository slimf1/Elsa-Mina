using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.ConnectFour;

[NamedCommand("end-connect-four", Aliases = ["c4end", "c4-end"])]
public class EndConnectFour : Command
{
    private readonly IRoomsManager _roomsManager;

    public EndConnectFour(IRoomsManager roomsManager)
    {
        _roomsManager = roomsManager;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override Task Run(IContext context)
    {
        var room = _roomsManager.GetRoom(context.RoomId);
        if (room?.Game is IConnectFourGame connectFourGame)
        {
            connectFourGame.Cancel();
            context.ReplyLocalizedMessage("c4_game_cancelled");
        }
        else
        {
            context.ReplyLocalizedMessage("c4_game_ongoing_game");
        }
        
        return Task.CompletedTask;
    }
}