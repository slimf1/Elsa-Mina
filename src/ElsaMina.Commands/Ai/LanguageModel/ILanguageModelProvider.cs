namespace ElsaMina.Commands.Ai.LanguageModel;

public interface ILanguageModelProvider
{
    Task<string> AskLanguageModelAsync(string prompt, CancellationToken cancellationToken = default);
    Task<string> AskLanguageModelAsync(LanguageModelRequest request, CancellationToken cancellationToken = default);
}