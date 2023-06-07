using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templating;

namespace ElsaMina.Commands.GuessingGame;

public class GuessingGameCommand : ICommand
{
    public static string Name => "guessinggame";

    public static IEnumerable<string> Aliases => new[]
    {
        "countriesgame"
    };

    public char RequiredRank => '+';

    private readonly IRoomsManager _roomsManager;
    private readonly ITemplatesManager _templatesManager;
    private readonly IRandomService _randomService;
    private readonly IConfigurationManager _configurationManager;

    public GuessingGameCommand(IRoomsManager roomsManager,
        ITemplatesManager templatesManager,
        IRandomService randomService,
        IConfigurationManager configurationManager)
    {
        _roomsManager = roomsManager;
        _templatesManager = templatesManager;
        _randomService = randomService;
        _configurationManager = configurationManager;
    }

    public Task Run(IContext context)
    {
        if (!int.TryParse(context.Target, out var count))
        {
            context.Reply("Please specify the number of turns.");
            return Task.CompletedTask;
        }

        if (count < 0 || count > 20)
        {
            context.Reply("Invalid number of turns (should be between 1 and 20)");
            return Task.CompletedTask;
        }

        var room = _roomsManager.GetRoom(context.RoomId);
        if (room.Game != null)
        {
            context.Reply("A game is already running");
            return Task.CompletedTask;
        }
        
        GuessingGame game = context.Command switch
        {
            "countriesgame" => new CountriesGame(context, _templatesManager, _randomService, room,
                _configurationManager, count),
            _ => null
        };
        if (game == null)
        {
            context.Reply("Invalid command");
            return Task.CompletedTask;
        }

        room.Game = game;
        game.Start();
        return Task.CompletedTask;
    }
}