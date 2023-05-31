using Newtonsoft.Json;

namespace ElsaMina.Core.Services.UserData;

public class UserDetailsDto
{
    [JsonProperty("isPrivate")]
    public string Id { get; set; }
    [JsonProperty("userid")]
    public string UserId { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("avatar")]
    public string Avatar { get; set; }
    [JsonProperty("group")]
    public string Group { get; set; }
    [JsonProperty("friended")]
    public bool Friended { get; set; }
    [JsonProperty("autoconfirmed")]
    public bool AutoConfirmed { get; set; }
    //  when user has no rooms => false / otherwise it's a dictionary 🤡
    // [JsonProperty("rooms")]
    // public IDictionary<string, UserDetailsRoomDto> Rooms { get; set; }
}