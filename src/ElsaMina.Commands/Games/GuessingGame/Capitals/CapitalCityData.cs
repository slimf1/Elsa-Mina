using Newtonsoft.Json;

namespace ElsaMina.Commands.Games.GuessingGame.Capitals;

public class CapitalCityData
{
    [JsonProperty("country_en")]
    public string CountryEnglish { get; set; }
    [JsonProperty("capital_en")]
    public string CapitalEnglish { get; set; }
    [JsonProperty("country_fr")]
    public string CountryFrench { get; set; }
    [JsonProperty("capital_fr")]
    public string CapitalFrench { get; set; }
}