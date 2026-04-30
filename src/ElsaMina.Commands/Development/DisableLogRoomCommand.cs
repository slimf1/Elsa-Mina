using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Development;

[NamedCommand("disablelogroom")]
public class DisableLogRoomCommand : DevelopmentCommand
{
    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        ShowdownSink.BotSender = null;
        ShowdownSink.RoomId = null;
        context.Reply("Showdown logging sink disabled.");
        return Task.CompletedTask;
    }
}
