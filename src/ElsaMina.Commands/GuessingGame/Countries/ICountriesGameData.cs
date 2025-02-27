namespace ElsaMina.Commands.GuessingGame.Countries;

public interface ICountriesGameData
{
    IEnumerable<CountryData> Countries { get; }
}