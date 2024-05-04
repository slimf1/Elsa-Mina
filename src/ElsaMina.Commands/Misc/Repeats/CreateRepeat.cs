using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Repeats;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Misc.Repeats;

[NamedCommand("repeat", Aliases = ["create-repeat"])]
public class CreateRepeat : Command
{
    private readonly IRepeatsManager _repeatsManager;

    public CreateRepeat(IRepeatsManager repeatsManager)
    {
        _repeatsManager = repeatsManager;
    }

    public override char RequiredRank => '+';

    public override string HelpMessageKey => "aboutrepeat_helpmessage";

    public override Task Run(IContext context)
    {
        string repeatId;
        string message;
        uint intervalInMinutes;
        try
        {
            var parts = context.Target.Split(",");
            repeatId = parts[0].ToLowerAlphaNum();
            message = parts[1].Trim();
            intervalInMinutes = uint.Parse(parts[2]);
        }
        catch (Exception)
        {
            ReplyLocalizedHelpMessage(context);
            return Task.CompletedTask;
        }

        var repeat = _repeatsManager.StartRepeat(context, repeatId, message, intervalInMinutes);
        if (repeat == null)
        {
            context.Reply("Impossible de démarrer le repeat. Vérifiez qu'un repeat avec la même id n'existe pas déjà.");
            return Task.CompletedTask;
        }

        context.Reply($"Un repeat a été démarré. Le message va être répété toutes les {intervalInMinutes} minute(s).");
        return Task.CompletedTask;
    }
}