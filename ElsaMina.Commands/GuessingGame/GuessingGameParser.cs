using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Commands.Parsers;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.GuessingGame;

public class GuessingGameParser : ChatMessageParser
{
    private readonly IRoomsManager _roomsManager;
    
    public GuessingGameParser(IDependencyContainerService dependencyContainerService,
        IRoomsManager roomsManager)
        : base(dependencyContainerService)
    {
        _roomsManager = roomsManager;
    }
    
    protected override Task HandleChatMessage(IContext context)
    {
        var room = _roomsManager.GetRoom(context.RoomId);
        if (room?.Game is CountriesGame countriesGame)
        {
            countriesGame.OnAnswer(context.Sender.Name, context.Target);
        }

        return Task.CompletedTask;
    }
}