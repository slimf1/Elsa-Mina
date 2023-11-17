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

    public override async Task Run(IContext context)
    {
        string repeatId = null;
        string message = null;
        var repeat = _repeatsManager.StartRepeat(context, "id", "lol test", 1);
    }
}