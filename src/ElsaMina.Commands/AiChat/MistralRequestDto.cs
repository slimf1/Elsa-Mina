using Newtonsoft.Json;

namespace ElsaMina.Commands.AiChat;

public class MistralRequestDto
{
    [JsonProperty("model")]
    public string Model { get; set; }
    [JsonProperty("messages")]
    public List<MistralRequestMessageDto> Messages { get; set; }
}

public class MistralRequestMessageDto
{
    [JsonProperty("role")]
    public string Role { get; set; }
    [JsonProperty("content")]
    public string Content { get; set; }
}