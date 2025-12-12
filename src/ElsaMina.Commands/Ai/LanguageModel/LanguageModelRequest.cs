namespace ElsaMina.Commands.Ai.LanguageModel;

public class LanguageModelRequest
{
    public string SystemPrompt { get; set; }
    public List<LanguageModelMessage> InputConversation { get; set; }
}