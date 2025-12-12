using Newtonsoft.Json;

namespace ElsaMina.Commands.Ai.LanguageModel.Google;

public class GeminiResponseDto
{
    [JsonProperty("candidates")]
    public List<Candidate> Candidates { get; set; }

    [JsonProperty("usageMetadata")]
    public UsageMetadata UsageMetadata { get; set; }

    [JsonProperty("modelVersion")]
    public string ModelVersion { get; set; }

    [JsonProperty("responseId")]
    public string ResponseId { get; set; }
}

public class Candidate
{
    [JsonProperty("content")]
    public CandidateContent Content { get; set; }

    [JsonProperty("finishReason")]
    public string FinishReason { get; set; }

    [JsonProperty("index")]
    public int Index { get; set; }
}

public class CandidateContent
{
    [JsonProperty("parts")]
    public List<CandidatePart> Parts { get; set; }

    [JsonProperty("role")]
    public string Role { get; set; }
}

public class CandidatePart
{
    [JsonProperty("text")]
    public string Text { get; set; }
}

public class UsageMetadata
{
    [JsonProperty("promptTokenCount")]
    public int PromptTokenCount { get; set; }

    [JsonProperty("candidatesTokenCount")]
    public int CandidatesTokenCount { get; set; }

    [JsonProperty("totalTokenCount")]
    public int TotalTokenCount { get; set; }

    [JsonProperty("promptTokensDetails")]
    public List<PromptTokensDetail> PromptTokensDetails { get; set; }

    [JsonProperty("thoughtsTokenCount")]
    public int ThoughtsTokenCount { get; set; }
}

public class PromptTokensDetail
{
    [JsonProperty("modality")]
    public string Modality { get; set; }

    [JsonProperty("tokenCount")]
    public int TokenCount { get; set; }
}
