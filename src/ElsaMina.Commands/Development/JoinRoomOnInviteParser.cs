using ElsaMina.Core.Commands.Parsers;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Commands.Development;

public class JoinRoomOnInviteParser : PrivateMessageParser

{
    public JoinRoomOnInviteParser(IDependencyContainerService dependencyContainerService)
        : base(dependencyContainerService)
    {
    }

    public override string Identifier => nameof(PrivateMessageParser);

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