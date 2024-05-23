using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.GuessingGame;

public class GuessingGameHandler : ChatMessageHandler
{
    private readonly IRoomsManager _roomsManager;
    
    public GuessingGameHandler(IContextFactory contextFactory,
        IRoomsManager roomsManager)
        : base(contextFactory)
    {
        _roomsManager = roomsManager;
    }

    public override string Identifier => nameof(ChatMessageHandler);

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