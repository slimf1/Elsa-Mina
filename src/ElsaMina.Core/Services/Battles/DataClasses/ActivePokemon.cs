using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Battles.DataClasses;

public sealed class ActivePokemon
{
    [JsonProperty("moves")]
    public List<Move> Moves { get; set; } = new();

    [JsonProperty("canTerastallize")]
    public string CanTerastallize { get; set; } = "";
}
