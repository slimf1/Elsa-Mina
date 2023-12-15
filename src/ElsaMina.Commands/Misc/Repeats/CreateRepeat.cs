using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Repeats;

namespace ElsaMina.Commands.Misc.Repeats;

public class CreateRepeat : Command<CreateRepeat>, INamed
{
    public static string Name => "repeat";
    public static IEnumerable<string> Aliases => new[] { "create-repeat" };

    private readonly IRepeatsManager _repeatsManager;

    public CreateRepeat(IRepeatsManager repeatsManager)
    {
        _repeatsManager = repeatsManager;
    }

    public override char RequiredRank => '+';

    public override Task Run(IContext context)
    {
        string repeatId;
        string message;
        uint intervalInMinutes;
        try
        {
            var parts = context.Target.Split(",");
            repeatId = parts[0];
            message = parts[1];
            intervalInMinutes = uint.Parse(parts[2]);
        }
        catch (Exception)
        {
            context.Reply("Syntaxe: -repeat <id>, <message>, <durée en minutes>");
            return Task.CompletedTask;
        }
        
        var repeat = _repeatsManager.StartRepeat(context, repeatId, message, intervalInMinutes);
        if (repeat == null)
        {
            context.Reply("Impossible de démarrer le repeat.");
            return Task.CompletedTask;
        }
        
        context.Reply("Un repeat a été démarré. Le message va être répété toutes les "); 
        return Task.CompletedTask;
    }
}