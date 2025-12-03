using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Development;

[NamedCommand("help", Aliases = ["about"])]
public class HelpCommand : Command
{
    private readonly IVersionProvider _versionProvider;

    public HelpCommand(IVersionProvider versionProvider)
    {
        _versionProvider = versionProvider;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override Rank RequiredRank => Rank.Regular;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        context.ReplyRankAwareLocalizedMessage("help", _versionProvider.Version);
        return Task.CompletedTask;
    }
}