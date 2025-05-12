using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Repeats;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Repeats;

[NamedCommand("startrepeat")]
public class StartRepeatCommand : Command
{
    private readonly IRepeatsManager _repeatsManager;

    public StartRepeatCommand(IRepeatsManager repeatsManager)
    {
        _repeatsManager = repeatsManager;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override bool IsPrivateMessageOnly => true;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        string roomId;
        string message;
        uint delayInMinutes;
        try
        {
            var parts = context.Target.Split(',');
            roomId = parts[0].ToLowerAlphaNum();
            message = parts[1].Trim();
            delayInMinutes = uint.Parse(parts[2]);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Could not parse repeat arguments");
            context.ReplyLocalizedMessage("repeat_start_error");
            return;
        }
        
        if (!await context.HasSufficientRankInRoom(roomId, Rank.Driver, cancellationToken))
        {
            context.ReplyLocalizedMessage("repeat_start_rank");
            return;
        }

        _repeatsManager.StartRepeat(context, roomId, message, TimeSpan.FromMinutes(delayInMinutes));
        context.ReplyLocalizedMessage("repeat_start_success");
    }
}