using Newtonsoft.Json;

namespace ElsaMina.Core.Services.UserData;

public class UserDataRankingDto
{
    [JsonProperty("elo")]
    public double Elo { get; set; }
    [JsonProperty("gxe")]
    public double Gxe { get; set; }
    [JsonProperty("rpr")]
    public double Rpr { get; set; }
    [JsonProperty("rprd")]
    public double Rprd { get; set; }
}