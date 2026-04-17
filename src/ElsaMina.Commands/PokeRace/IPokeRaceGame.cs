using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Games;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.PokeRace;

public interface IPokeRaceGame : IGame
{
    int GameId { get; }
    IContext Context { get; set; }
    IReadOnlyDictionary<string, (string Name, string Pokemon)> Players { get; }
    void BeginJoinPhase();
    (bool Success, string MessageKey, object[] Args) JoinRace(string userName, string pokemonName);
    Task StartRaceAsync();
    void Cancel();
}