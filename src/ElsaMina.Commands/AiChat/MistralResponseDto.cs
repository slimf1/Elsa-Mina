using Newtonsoft.Json;

namespace ElsaMina.Commands.AiChat;

public class MistralResponseDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("object")]
    public string Object { get; set; }
    [JsonProperty("created")]
    public long Created { get; set; }
    [JsonProperty("model")]
    public string Model { get; set; }
    [JsonProperty("choices")]
    public List<MistralChoiceDto> Choices { get; set; }
}

public class MistralChoiceDto
{
    [JsonProperty("index")]
    public int Index { get; set; }
    [JsonProperty("message")]
    public MistralResponseMessageDto Message { get; set; }
}

public class MistralResponseMessageDto
{
    [JsonProperty("role")]
    public string Role { get; set; }
    [JsonProperty("content")]
    public string Content { get; set; }
    [JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
}