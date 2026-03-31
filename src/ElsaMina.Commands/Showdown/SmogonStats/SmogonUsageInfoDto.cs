using Newtonsoft.Json;

namespace ElsaMina.Commands.Showdown.SmogonStats;

public class SmogonUsageInfoDto
{
    [JsonProperty("metagame")]
    public string Metagame { get; set; }

    [JsonProperty("cutoff")]
    public double Cutoff { get; set; }

    [JsonProperty("cutoff deviation")]
    public double CutoffDeviation { get; set; }

    [JsonProperty("team type")]
    public string TeamType { get; set; }

    [JsonProperty("number of battles")]
    public int NumberOfBattles { get; set; }
}
