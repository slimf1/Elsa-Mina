using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templating;

namespace ElsaMina.Commands.GuessingGame;

public class GuessingGameCommand : ICommand
{
    public static string Name => "guessinggame";

    public static IEnumerable<string> Aliases => new[]
    {
        "countriesgame", "countries-game"
    };

    public bool IsPrivateMessageOnly { get; }
    public char RequiredRank { get; }

    private readonly IRoomsManager _roomsManager;
    private readonly ITemplatesManager _templatesManager;
    private readonly IRandomService _randomService; 

    public GuessingGameCommand(IRoomsManager roomsManager,
        ITemplatesManager templatesManager,
        IRandomService randomService)
    {
        _roomsManager = roomsManager;
        _templatesManager = templatesManager;
        _randomService = randomService;
    }

    public Task Run(IContext context)
    {
        int count;
        if (!int.TryParse(context.Target, out count))
        {
            context.Reply("lol errror XDDDDDDDDDDDDDDD");
            return;
        }

        if (count < 0 || count > 20)
        {
            context.Reply("eeeeeeeeeeeeee");
            return;
        }

        var room = _roomsManager.GetRoom(context.RoomId);
        var a = new CountriesGame(context, _templatesManager, _randomService, count);
        
    }
}