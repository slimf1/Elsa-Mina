using Newtonsoft.Json;

namespace ElsaMina.Core.Utils;

public class PokemonSet
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("species")]
    public string Species { get; set; }
    [JsonProperty("gender")]
    public string Gender { get; set; }
    [JsonProperty("item")]
    public string Item { get; set; }
    [JsonProperty("ability")]
    public string Ability { get; set; }
    [JsonProperty("shiny")]
    public bool IsShiny { get; set; }
    [JsonProperty("level")]
    public int Level { get; set; }
    [JsonProperty("happiness")]
    public int Happiness { get; set; } = -1;
    [JsonProperty("pokeball")]
    public string Pokeball { get; set; }
    [JsonProperty("hpType")]
    public string HiddenPowerType { get; set; }
    [JsonProperty("teraType")]
    public string TeraType { get; set; }
    [JsonProperty("dynamaxLevel")]
    public int DynamaxLevel { get; set; } = -1;
    [JsonProperty("gigantamax")]
    public bool IsGigantamax { get; set; }
    [JsonProperty("nature")]
    public string Nature { get; set; }
    [JsonProperty("evs")]
    public IDictionary<string, int> EffortValues { get; set; }
    [JsonProperty("ivs")]
    public IDictionary<string, int> IndividualValues { get; set; }
    [JsonProperty("moves")]
    public ICollection<string> Moves { get; set; }
}