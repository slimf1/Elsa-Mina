using ElsaMina.Core.Commands.Parsers;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

public class JoinRoomOnInviteParser : PrivateMessageParser

{
    public JoinRoomOnInviteParser(IContextFactory contextFactory)
        : base(contextFactory)
    {
    }

    public override string Identifier => nameof(PrivateMessageParser);

    protected override Task HandleMessage(IContext context)
    {
        if (!context.Message.StartsWith("/invite ") || !context.IsSenderWhitelisted)
        {
            return Task.CompletedTask;
        }

        var roomToJoin = context.Message[8..];
        context.Reply($"/join {roomToJoin}");
        return Task.CompletedTask;
    }
}