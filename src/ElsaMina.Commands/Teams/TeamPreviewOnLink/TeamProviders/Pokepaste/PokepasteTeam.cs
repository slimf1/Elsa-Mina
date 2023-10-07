using Newtonsoft.Json;

namespace ElsaMina.Commands.Teams.TeamPreviewOnLink.TeamProviders.Pokepaste;

public class PokepasteTeam
{
    [JsonProperty("author")]
    public string Author { get; set; }
    [JsonProperty("notes")]
    public string Notes { get; set; }
    [JsonProperty("paste")]
    public string Paste { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
}