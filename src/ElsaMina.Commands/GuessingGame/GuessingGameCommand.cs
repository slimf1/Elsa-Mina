using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.GuessingGame;

[NamedCommand("guessinggame", Aliases = ["countriesgame"])]
public class GuessingGameCommand : Command
{
    private const int MAX_TURNS_COUNT = 20;
    
    private readonly IRoomsManager _roomsManager;
    private readonly IDependencyContainerService _dependencyContainerService;

    public GuessingGameCommand(IRoomsManager roomsManager,
        IDependencyContainerService dependencyContainerService)
    {
        _roomsManager = roomsManager;
        _dependencyContainerService = dependencyContainerService;
    }

    public override char RequiredRank => '+';

    public override Task OnBotStartUp()
    {
        return CountriesGame.LoadCountriesGameData();
    }

    // TODO : localize
    public override Task Run(IContext context)
    {
        if (!int.TryParse(context.Target, out var turnsCount))
        {
            context.Reply("Please specify the number of turns.");
            return Task.CompletedTask;
        }

        if (turnsCount is <= 0 or > MAX_TURNS_COUNT)
        {
            context.Reply($"Invalid number of turns (should be between 1 and {MAX_TURNS_COUNT})");
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
        game.CleanupAction = () => room.EndGame();

        room.Game = game;
        game.Start();
        return Task.CompletedTask;
    }
}