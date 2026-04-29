namespace ElsaMina.Commands.Games.GuessingGame.Countries;

public interface ICountriesGameData
{
    IEnumerable<CountryData> Countries { get; }
}