using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Commands.Development;

[NamedCommand("help", Aliases = ["about"])]
public class Help : Command
{
    private readonly IVersionProvider _versionProvider;

    public Help(IVersionProvider versionProvider)
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