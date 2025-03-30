using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Commands.GuessingGame.PokeCries;
using ElsaMina.Commands.GuessingGame.PokeDesc;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Commands.GuessingGame;

[NamedCommand("guessinggame", Aliases = ["countriesgame", "pokedesc", "pokecries"])]
public class GuessingGameCommand : Command
{
    private const int MAX_TURNS_COUNT = 20;
    
    private readonly IDependencyContainerService _dependencyContainerService;

    public GuessingGameCommand(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(context.Target, out var turnsCount))
        {
            context.ReplyLocalizedMessage("guessing_game_specify");
            return;
        }

        if (turnsCount is <= 0 or > MAX_TURNS_COUNT)
        {
            context.ReplyLocalizedMessage("guessing_game_invalid_number_turns", MAX_TURNS_COUNT);
            return;
        }

        var room = context.Room;
        if (room.Game != null)
        {
            context.ReplyLocalizedMessage("guessing_game_currently_ongoing");
            return;
        }

        GuessingGame game = context.Command switch
        {
            "countriesgame" => _dependencyContainerService.Resolve<CountriesGame>(),
            "pokedesc" => _dependencyContainerService.Resolve<PokeDescGame>(),
            "pokecries" => _dependencyContainerService.Resolve<PokeCriesGame>(),
            _ => null
        };
        if (game == null)
        {
            context.ReplyLocalizedMessage("guessing_game_invalid_command");
            return;
        }

        game.TurnsCount = turnsCount;
        game.Context = context;

        room.Game = game;
        await game.Start();
    }
}