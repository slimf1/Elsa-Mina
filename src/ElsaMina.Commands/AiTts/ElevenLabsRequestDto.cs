using Newtonsoft.Json;

namespace ElsaMina.Commands.AiTts;

public class ElevenLabsRequestDto
{
    [JsonProperty("text")]
    public string Text { get; set; }
    
    [JsonProperty("model_id")]
    public string ModelId { get; set; }
}