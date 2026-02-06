using Newtonsoft.Json;

namespace ElsaMina.Commands.Ai.LanguageModel.OpenAi;

public class GptRequestDto
{
    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("items")]
    public List<GptConversationItemDto> Messages { get; set; }
}

public class GptConversationItemDto
{

    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}
