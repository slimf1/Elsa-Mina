using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Battles.Dtos;

public sealed class BattleStateDto
{
    [JsonProperty("active")]
    public List<ActivePokemon> Active { get; set; } = new();

    [JsonProperty("forceSwitch")]
    [JsonConverter(typeof(ForceSwitchConverter))]
    public List<bool> ForceSwitch { get; set; } = new();

    [JsonProperty("teamPreview", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool TeamPreview { get; set; }
    
    [JsonProperty("wait", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool Wait { get; set; }

    [JsonProperty("side")] 
    public Side Side { get; set; } = new();

    [JsonProperty("rqid")] 
    public int Rqid { get; set; }
}