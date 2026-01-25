using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Battles.DataClasses;


public sealed class Side
{
    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("pokemon")]
    public List<SidePokemon> Pokemon { get; set; } = new();
}