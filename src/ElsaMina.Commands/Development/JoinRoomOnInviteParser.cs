using ElsaMina.Core.Bot;
using ElsaMina.Core.Commands.Parsers;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.PrivateMessages;

namespace ElsaMina.Commands.Development;

public class JoinRoomOnInviteParser : PrivateMessageParser

{
    public JoinRoomOnInviteParser(IContextFactory contextFactory, IBot bot, IPmSendersManager pmSendersManager)
        : base(contextFactory, bot, pmSendersManager)
    {
    }

    protected override Task HandlePrivateMessage(IContext context)
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