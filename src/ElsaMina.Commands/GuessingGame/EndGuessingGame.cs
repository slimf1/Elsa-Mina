using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.GuessingGame;

[NamedCommand("endguessinggame", Aliases = ["endcountriesgame"])]
public class EndGuessingGame : Command
{
    private readonly IRoomsManager _roomsManager;

    public EndGuessingGame(IRoomsManager roomsManager)
    {
        _roomsManager = roomsManager;
    }

    public override char RequiredRank => '+';

    public override Task Run(IContext context)
    {
        var room = _roomsManager.GetRoom(context.RoomId);
        if (room?.Game is GuessingGame guessingGame)
        {
            guessingGame.Cancel();
            context.ReplyLocalizedMessage("end_guessing_game_success");
            return Task.CompletedTask;
        }

        context.ReplyLocalizedMessage("end_guessing_game_no_game");
        return Task.CompletedTask;
    }
}