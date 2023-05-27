namespace ElsaMina.Core.Commands.Parsers;

public abstract class ChatMessageParser : Parser
{
    public override async Task Execute(string[] parts)
    {
        if (parts.Length > 1 && parts[1] == "c:")
        {
            await HandleChatMessage(long.Parse(parts[2]), parts[3], parts[4]);
        }
    }

    protected abstract Task HandleChatMessage(long timestamp, string user, string message);
}