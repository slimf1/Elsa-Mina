using Newtonsoft.Json;

namespace ElsaMina.Commands.GuessingGame.Countries;

public class CountryData
{
    [JsonProperty("english_name")]
    public string EnglishName { get; set; }
    [JsonProperty("french_name")]
    public string FrenchName { get; set; }
    [JsonProperty("flag")]
    public string Flag { get; set; }
    [JsonProperty("location")]
    public string Location { get; set; }
}