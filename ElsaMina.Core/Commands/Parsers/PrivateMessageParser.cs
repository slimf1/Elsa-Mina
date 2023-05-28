using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Commands.Parsers;

public abstract class PrivateMessageParser : Parser
{
    public override async Task Execute(string[] parts)
    {
        if (parts.Length > 2 && parts[1] == "pm")
        {
            // await HandlePrivateMessage(parts[2], parts[4]);
        }
    }

    protected abstract Task HandlePrivateMessage(IContext context);
}