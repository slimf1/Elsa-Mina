using Newtonsoft.Json;

namespace ElsaMina.Core.Services.UserDetails;

public class UserDetailsRoomDto
{
    [JsonProperty("isPrivate")]
    public bool IsPrivate { get; set; }
}