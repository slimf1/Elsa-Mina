using ElsaMina.Commands.GuessingGame.Capitals;
using ElsaMina.Commands.GuessingGame.Countries;
using ElsaMina.Commands.GuessingGame.PokeDesc;

namespace ElsaMina.Commands;

public interface IDataManager
{
    ICountriesGameData CountriesGameData { get; }
    IReadOnlyList<PokemonDescription> PokemonDescriptions { get; }
    ICapitalCitiesGameData CapitalCitiesGameData { get; }
}