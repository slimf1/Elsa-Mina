using Newtonsoft.Json;

namespace ElsaMina.Commands.Teams.TeamProviders.Showdown;

public class ShowdownTeamDto
{
    [JsonProperty("team")]
    public string PackedTeam { get; set; }
    
    [JsonProperty("ownerid")]
    public string OwnerId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }
}