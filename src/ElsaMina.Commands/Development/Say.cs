using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

[NamedCommand("say")]
public class Say : DevelopmentCommand
{
    private readonly IBot _bot;

    public Say(IBot bot)
    {
        _bot = bot;
    }

    public override bool IsPrivateMessageOnly => true;

    public override Task Run(IContext context)
    {
        var parts = context.Target.Split(';');
        Log.Information("Say command used: {0}", context.Target);
        _bot.Say(parts[0], parts[1]);
        return Task.CompletedTask;
    }
}