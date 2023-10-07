using Newtonsoft.Json;

namespace ElsaMina.Commands.Teams.TeamPreviewOnLink.TeamProviders.CoupCritique;

public class CoupCritiqueTeam
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("export")]
    public string Export { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("user")]
    public CoupCritiqueUser User { get; set; }
}