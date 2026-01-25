using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Battles.DataClasses;

public sealed class BattleState
{
    [JsonProperty("active")]
    public List<ActivePokemon> Active { get; set; } = new();

    [JsonProperty("forceSwitch")]
    [JsonConverter(typeof(ForceSwitchConverter))]
    public List<bool> ForceSwitch { get; set; } = new();
    
    [JsonProperty("teamPreview", DefaultValueHandling =  DefaultValueHandling.Ignore)]
    public bool TeamPreview { get; set; }

    [JsonProperty("side")]
    public Side Side { get; set; } = new();

    [JsonProperty("rqid")]
    public int Rqid { get; set; }
}
