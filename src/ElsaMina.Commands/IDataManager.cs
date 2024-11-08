using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Commands.GuessingGame.PokeDesc;

namespace ElsaMina.Commands;

public interface IDataManager
{
    CountriesGameData CountriesGameData { get; }
    IReadOnlyList<PokemonDescription> PokemonDescriptions { get; }
}