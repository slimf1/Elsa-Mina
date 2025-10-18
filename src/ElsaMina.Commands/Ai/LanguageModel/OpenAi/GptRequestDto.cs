using Newtonsoft.Json;

namespace ElsaMina.Commands.Ai.LanguageModel.OpenAi;

public class GptRequestDto
{
    [JsonProperty("model")]
    public string Model { get; set; }
    [JsonProperty("input")]
    public string Input { get; set; }
}