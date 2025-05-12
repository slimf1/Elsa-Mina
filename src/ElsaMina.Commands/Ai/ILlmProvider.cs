namespace ElsaMina.Commands.Ai;

public interface ILlmProvider
{
    public Task<string> AskLlmAsync(string prompt, CancellationToken cancellationToken = default);
}