using Newtonsoft.Json;

namespace ElsaMina.Commands.Ai.Tts;

public class ElevenLabsRequestDto
{
    [JsonProperty("text")]
    public string Text { get; set; }
    
    [JsonProperty("model_id")]
    public string ModelId { get; set; }
}