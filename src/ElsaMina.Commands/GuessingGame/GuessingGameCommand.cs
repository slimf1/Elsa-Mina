using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.GuessingGame;

public class GuessingGameCommand : Command<GuessingGameCommand>, INamed
{
    public static string Name => "guessinggame";

    public static IEnumerable<string> Aliases => new[]
    {
        "countriesgame"
    };

    public override char RequiredRank => '+';

    private readonly IRoomsManager _roomsManager;
    private readonly IDependencyContainerService _dependencyContainerService;

    public GuessingGameCommand(IRoomsManager roomsManager,
        IDependencyContainerService dependencyContainerService)
    {
        _roomsManager = roomsManager;
        _dependencyContainerService = dependencyContainerService;
    }

    public override Task Run(IContext context)
    {
        if (!int.TryParse(context.Target, out var turnsCount))
        {
            context.Reply("Please specify the number of turns.");
            return Task.CompletedTask;
        }

        if (turnsCount is < 0 or > 20)
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
            "countriesgame" => _dependencyContainerService.Resolve<CountriesGame>(),
            _ => null
        };
        if (game == null)
        {
            context.Reply("Invalid command");
            return Task.CompletedTask;
        }

        game.TurnsCount = turnsCount;
        game.Room = room;
        game.Context = context;
        
        room.Game = game;
        game.Start();
        return Task.CompletedTask;
    }
}