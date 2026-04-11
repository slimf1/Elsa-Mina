using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using Lusamine.GTranslate;

namespace ElsaMina.Commands.Misc.Translation;

[NamedCommand("googletranslate", Aliases = ["ggltrans", "googletrans", "gt"])]
public class GoogleTranslateCommand : Command
{
    public override Rank RequiredRank => Rank.Regular;
    public override bool IsAllowedInPrivateMessage => true;
    public override string HelpMessageKey => "googletranslate_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var parts = context.Target.Split(',');
        if (parts.Length is not (2 or 3))
        {
            context.ReplyLocalizedMessage(HelpMessageKey);
            return;
        }

        var text = parts[0].Trim();
        var dest = parts[1].Trim().ToLowerInvariant();
        var src = parts.Length == 3 ? parts[2].Trim().ToLowerInvariant() : "auto";

        try
        {
            await using var translator = new Translator();
            var result = await translator.TranslateAsync(text, dest, src, cancellationToken);
            context.Reply($"**Translation:** {result.Text}", rankAware: true);
        }
        catch (Exception)
        {
            context.ReplyLocalizedMessage("googletranslate_error");
        }
    }
}
