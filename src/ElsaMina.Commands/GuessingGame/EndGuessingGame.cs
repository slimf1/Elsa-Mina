﻿using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.GuessingGame;

public class EndGuessingGame : Command<EndGuessingGame>, INamed
{
    public static string Name => "endguessinggame";
    public static List<string> Aliases => ["endcountriesgame"];
    public override char RequiredRank => '+';

    private readonly IRoomsManager _roomsManager;

    public EndGuessingGame(IRoomsManager roomsManager)
    {
        _roomsManager = roomsManager;
    }

    public override Task Run(IContext context)
    {
        var room = _roomsManager.GetRoom(context.RoomId);
        if (room?.Game is GuessingGame)
        {
            room.EndGame();
            context.ReplyLocalizedMessage("end_guessing_game_success");
            return Task.CompletedTask;
        }
        
        context.ReplyLocalizedMessage("end_guessing_game_no_game");
        return Task.CompletedTask;
    }
}