using ElsaMina.Core.Contexts;
using ElsaMina.Core.Handlers.DefaultHandlers;

namespace ElsaMina.Commands.Development;

public class JoinRoomOnInviteHandler : PrivateMessageHandler
{
    public JoinRoomOnInviteHandler(IContextFactory contextFactory)
        : base(contextFactory)
    {
    }

    public override Task HandleMessageAsync(IContext context, CancellationToken cancellationToken = default)
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