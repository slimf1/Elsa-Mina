using Newtonsoft.Json;

namespace ElsaMina.Commands.Games.GuessingGame.Countries;

public class CountriesGameData : ICountriesGameData
{
    [JsonProperty("values")]
    public IEnumerable<CountryData> Countries { get; set; }
}