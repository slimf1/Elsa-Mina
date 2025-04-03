using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Repeats;

namespace ElsaMina.Commands.Repeats;

[NamedCommand("startrepeat")]
public class StartRepeatCommand : Command
{
    private readonly IRepeatsManager _repeatsManager;

    public StartRepeatCommand(IRepeatsManager repeatsManager)
    {
        _repeatsManager = repeatsManager;
    }

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        string message;
        uint delayInHours;
        try
        {
            var parts = context.Target.Split(',');
            message = parts[0].Trim();
            delayInHours = uint.Parse(parts[1]);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Could not parse repeat arguments");
            return Task.CompletedTask;
        }

        var started = _repeatsManager.StartRepeat(context, message, TimeSpan.FromHours(delayInHours));
        if (started)
        {
            context.Reply("Repeat lancé avec succès (todo i18n)");
        }
        else
        {
            context.Reply("Le repeat n'a pas pu être lancé");
        }

        return Task.CompletedTask;
    }
}