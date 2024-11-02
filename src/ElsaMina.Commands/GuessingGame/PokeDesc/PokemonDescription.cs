using Newtonsoft.Json;

namespace ElsaMina.Commands.GuessingGame.PokeDesc;

public class PokemonDescription
{
    [JsonProperty]
    public string EnglishName { get; set; }
    [JsonProperty]
    public string FrenchName { get; set; }
    [JsonProperty]
    public string Description { get; set; }
}