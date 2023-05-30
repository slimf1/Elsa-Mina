using Newtonsoft.Json;

namespace ElsaMina.Core.Services.UserData;

public class UserDataRoomDto
{
    [JsonProperty("isPrivate")]
    public bool IsPrivate { get; set; }
}