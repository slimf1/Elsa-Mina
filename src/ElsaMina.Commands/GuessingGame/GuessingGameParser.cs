﻿using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Core.Commands.Parsers;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.GuessingGame;

public class GuessingGameParser : ChatMessageParser
{
    private readonly IRoomsManager _roomsManager;
    
    public GuessingGameParser(IContextFactory contextFactory,
        IRoomsManager roomsManager)
        : base(contextFactory)
    {
        _roomsManager = roomsManager;
    }

    public override string Identifier => nameof(ChatMessageParser);

    protected override Task HandleMessage(IContext context)
    {
        var room = _roomsManager.GetRoom(context.RoomId);
        if (room?.Game is CountriesGame countriesGame)
        {
            countriesGame.OnAnswer(context.Sender.Name, context.Message);
        }

        return Task.CompletedTask;
    }
}