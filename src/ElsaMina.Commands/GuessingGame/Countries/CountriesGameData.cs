using Newtonsoft.Json;

namespace ElsaMina.Commands.GuessingGame.Countries;

public class CountriesGameData
{
    [JsonProperty("values")]
    public IEnumerable<CountryData> Countries { get; set; }
}