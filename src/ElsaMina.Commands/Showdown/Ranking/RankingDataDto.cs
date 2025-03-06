using System.Drawing;
using ElsaMina.Core.Utils;
using Newtonsoft.Json;

namespace ElsaMina.Commands.Showdown.Ranking;

public class RankingDataDto
{
    [JsonProperty("formatid")]
    public string FormatId { get; set; }
    [JsonProperty("w")]
    public int Wins { get; set; }
    [JsonProperty("l")]
    public int Losses { get; set; }
    [JsonProperty("t")]
    public int Ties { get; set; }
    [JsonProperty("gxe")]
    public double Gxe { get; set; }
    [JsonProperty("elo")]
    public double Elo { get; set; }
    [JsonProperty("first_played", NullValueHandling = NullValueHandling.Ignore)]
    public int FirstPlayed { get; set; }
    [JsonProperty("last_played", NullValueHandling = NullValueHandling.Ignore)]
    public int LastPlayed { get; set; }
    
    public double WinRate => Wins + Losses == 0 ? 0 : 100 * Wins / (double)(Wins + Losses);
    public Color GxeBasedColor => ShowdownColors.FromHsl(Gxe * 1.05, 85, 45);

}