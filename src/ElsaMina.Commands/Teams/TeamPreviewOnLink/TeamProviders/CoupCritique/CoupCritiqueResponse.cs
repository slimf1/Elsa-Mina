using Newtonsoft.Json;

namespace ElsaMina.Commands.Teams.TeamPreviewOnLink.TeamProviders.CoupCritique;

public class CoupCritiqueResponse
{
    [JsonProperty("team")]
    public CoupCritiqueTeam Team { get; set; }
}