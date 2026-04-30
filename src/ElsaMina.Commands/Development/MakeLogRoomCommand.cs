using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Utils;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Development;

[NamedCommand("makelogroom")]
public class MakeLogRoomCommand : DevelopmentCommand
{
    private readonly IBot _bot;
    private readonly IConfiguration _configuration;

    public MakeLogRoomCommand(IBot bot, IConfiguration configuration)
    {
        _bot = bot;
        _configuration = configuration;
    }

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var target = context.Target.Trim();

        if (!string.IsNullOrEmpty(target))
        {
            context.Reply($"/join {target}");
            ShowdownSink.BotSender = _bot.Say;
            ShowdownSink.RoomId = target;
            context.Reply($"Logging redirected to: {target}");
        }
        else
        {
            const string title = "logs";
            var roomId = $"groupchat-{_configuration.Name.ToLowerAlphaNum()}-{title.ToLowerAlphaNum()}";
            _bot.Say(context.RoomId, $"/makegroupchat {title}");
            ShowdownSink.BotSender = _bot.Say;
            ShowdownSink.RoomId = roomId;
            context.Reply($"Logging room created: {roomId}");
        }

        return Task.CompletedTask;
    }
}
