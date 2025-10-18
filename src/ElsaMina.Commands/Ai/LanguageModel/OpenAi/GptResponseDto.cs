using Newtonsoft.Json;

namespace ElsaMina.Commands.Ai.LanguageModel.OpenAi;

public class GptResponseDto
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("object")] public string Object { get; set; }

    [JsonProperty("created_at")] public long CreatedAt { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("background")] public bool Background { get; set; }

    [JsonProperty("model")] public string Model { get; set; }
    [JsonProperty("output")] public List<OutputItemDto> Output { get; set; }

    [JsonProperty("usage")] public UsageDto Usage { get; set; }
}

public class OutputItemDto
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("type")] public string Type { get; set; } // e.g. "message"

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("content")] public List<ContentItemDto> Content { get; set; }

    [JsonProperty("role")] public string Role { get; set; }
}

public class ContentItemDto
{
    [JsonProperty("type")] public string Type { get; set; } // e.g. "output_text"

    [JsonProperty("text")] public string Text { get; set; }
}

public class TextDto
{
    [JsonProperty("format")] public TextFormatDto Format { get; set; }

    [JsonProperty("verbosity")] public string Verbosity { get; set; }
}

public class TextFormatDto
{
    [JsonProperty("type")] public string Type { get; set; } // e.g. "text"
}

public class UsageDto
{
    [JsonProperty("input_tokens")] public int InputTokens { get; set; }

    [JsonProperty("input_tokens_details")] public TokenDetailsDto InputTokensDetails { get; set; }

    [JsonProperty("output_tokens")] public int OutputTokens { get; set; }

    [JsonProperty("output_tokens_details")]
    public OutputTokenDetailsDto OutputTokensDetails { get; set; }

    [JsonProperty("total_tokens")] public int TotalTokens { get; set; }
}

public class TokenDetailsDto
{
    [JsonProperty("cached_tokens")] public int CachedTokens { get; set; }
}

public class OutputTokenDetailsDto
{
    [JsonProperty("reasoning_tokens")] public int ReasoningTokens { get; set; }
}