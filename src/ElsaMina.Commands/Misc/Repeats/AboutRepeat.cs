using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Repeats;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Misc.Repeats;

public class AboutRepeat : Command<AboutRepeat>, INamed
{
    public static string Name => "repeat";
    public static IEnumerable<string> Aliases => new[] { "about-repeat" };

    private readonly IRepeatsManager _repeatsManager;

    public AboutRepeat(IRepeatsManager repeatsManager)
    {
        _repeatsManager = repeatsManager;
    }

    public override Task Run(IContext context)
    {
        var repeat = _repeatsManager.GetRepeat(context.RoomId, context.Target.ToLowerAlphaNum());
        if (repeat == null)
        {
            // TODO : strings
            return Task.CompletedTask;
        }
        
        context.Reply($"Un repeat a été lancé avec l'ID \"{repeat.RepeatId}\" ");
        return Task.CompletedTask;
    }
}