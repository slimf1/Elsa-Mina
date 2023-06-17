using Newtonsoft.Json;

namespace ElsaMina.Core.Services.UserData;

public class UserDataDto
{
    [JsonProperty("username")]
    public string UserName { get; set; }
    [JsonProperty("userid")]
    public string UserId { get; set; }
    [JsonProperty("registertime")]
    public long RegisterTime { get; set; }
    [JsonProperty("group")]
    public long Group { get; set; }
    [JsonProperty("ratings")]
    public IDictionary<string, UserDataRankingDto> Ratings { get; set; }
}