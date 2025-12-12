using Newtonsoft.Json;

namespace ElsaMina.Commands.Ai.LanguageModel.Google;

public class GeminiRequestDto
{
    [JsonProperty("system_instruction")]
    public SystemInstruction SystemInstruction { get; set; }

    [JsonProperty("contents")]
    public List<Content> Contents { get; set; }
}

public class SystemInstruction
{
    [JsonProperty("parts")]
    public List<InstructionPart> Parts { get; set; }
}

public class InstructionPart
{
    [JsonProperty("text")]
    public string Text { get; set; }
}

public class Content
{
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("parts")]
    public List<ContentPart> Parts { get; set; }
}

public class ContentPart
{
    [JsonProperty("text")]
    public string Text { get; set; }
}
