using Newtonsoft.Json;

namespace ElsaMina.Commands.Showdown.Ladder;

public class LadderDto
{
    [JsonProperty("formatid")]
    public string FormatId { get; set; }
    [JsonProperty("format")]
    public string Format { get; set; }
    [JsonProperty("toplist")]
    public IEnumerable<LadderPlayerDto> TopList { get; set; }
}

public class LadderPlayerDto
{
    [JsonProperty("userid")]
    public string UserId { get; set; }
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonProperty("w")]
    public int Wins { get; set; }
    [JsonProperty("l")]
    public int Losses { get; set; }
    [JsonProperty("t")]
    public int Ties { get; set; }
    [JsonProperty("elo")]
    public double Elo { get; set; }
    [JsonProperty("gxe")]
    public double Gxe { get; set; }

    public double WinRate => Wins + Losses == 0 ? 0 : 100 * Wins / (double)(Wins + Losses);
    public int Index { get; set; }
    public int InnerIndex { get; set; }
    public int? EloDifference { get; set; }
    public int? IndexDifference { get; set; }
    public int? InnerIndexDifference { get; set; }
}
