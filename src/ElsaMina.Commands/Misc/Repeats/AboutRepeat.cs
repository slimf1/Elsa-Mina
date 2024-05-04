using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Repeats;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Misc.Repeats;

[NamedCommand("show-repeat", Aliases = ["about-repeat", "show-repeat"])]
public class AboutRepeat : Command
{
    private readonly IRepeatsManager _repeatsManager;

    public AboutRepeat(IRepeatsManager repeatsManager)
    {
        _repeatsManager = repeatsManager;
    }

    public override char RequiredRank => '%';

    public override Task Run(IContext context)
    {
        var repeat = _repeatsManager.GetRepeat(context.RoomId, context.Target.ToLowerAlphaNum());
        if (repeat == null)
        {
            context.ReplyLocalizedMessage("aboutrepeat_not_found");
            return Task.CompletedTask;
        }
        
        context.SendHtml($"[Repeat] ID = \"{repeat.RepeatId}\", Message = \"{repeat.Message}\", Interval = {repeat.Interval}");
        return Task.CompletedTask;
    }
}