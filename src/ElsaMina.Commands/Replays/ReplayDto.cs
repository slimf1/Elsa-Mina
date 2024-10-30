using Newtonsoft.Json;

namespace ElsaMina.Commands.Replays;

public class ReplayDto
{
    [JsonProperty]
    public string Id { get; set; }
    [JsonProperty]
    public string Format { get; set; }
    [JsonProperty]
    public List<string> Players { get; set; } = [];
    [JsonProperty]
    public string Log { get; set; }
    [JsonProperty]
    public long UploadTime { get; set; }
    [JsonProperty]
    public int Views { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int Rating { get; set; }
    [JsonProperty]
    public string FormatId { get; set; }
    [JsonProperty]
    public int Private { get; set; }
    [JsonProperty]
    public string Password { get; set; }
}