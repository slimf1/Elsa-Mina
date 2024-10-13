using ElsaMina.Core.Handlers;

namespace ElsaMina.Commands.JoinPhrases;

public class JoinPhraseHandler : Handler
{
    public override string Identifier => nameof(JoinPhraseHandler);

    protected override Task HandleReceivedMessage(string[] parts, string roomId = null)
    {
        // todo
        return Task.CompletedTask;
    }
}