using Newtonsoft.Json;

namespace ElsaMina.Commands.Teams.TeamPreviewOnLink.TeamProviders.CoupCritique;

public class CoupCritiqueUser
{
    [JsonProperty("username")]
    public string UserName { get; set; }
}