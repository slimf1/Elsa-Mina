using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Misc.Pokemon;

[NamedCommand("afd", "afd-back")]
public class AfdSpriteCommand : Command
{
    private const string FRONT_SPRITE_URL = "https://play.pokemonshowdown.com/sprites/afd/{0}.png";
    private const string BACK_SPRITE_URL = "https://play.pokemonshowdown.com/sprites/afd-back/{0}.png";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.Target))
        {
            context.ReplyLocalizedMessage("pokemon_name_required");
            return Task.CompletedTask;
        }

        var pokemonName = context.Target.ToLowerInvariant();
        var isBackSprite = context.Command == "afd-back";
        var spriteUrl = string.Format(isBackSprite ? BACK_SPRITE_URL : FRONT_SPRITE_URL, pokemonName);
        var spriteType = isBackSprite ? "back" : "front";

        var imgHtml = $"""<img src="{spriteUrl}" width="80" height="80" alt="{pokemonName} {spriteType} sprite">""";
        context.SendHtmlIn(imgHtml, null, true);
        
        return Task.CompletedTask;
    }
}