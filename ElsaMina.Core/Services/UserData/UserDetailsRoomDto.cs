using Newtonsoft.Json;

namespace ElsaMina.Core.Services.UserData;

public class UserDetailsRoomDto
{
    [JsonProperty("isPrivate")]
    public bool IsPrivate { get; set; }
}