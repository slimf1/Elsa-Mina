using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Misc;

[NamedCommand("bugreport", "bug", "issue")]
public class BugReportCommand : Command
{
    private readonly IConfiguration _configuration;

    public BugReportCommand(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public override Rank RequiredRank => Rank.Regular;
    public override bool IsAllowedInPrivateMessage => true;
    public override string HelpMessageKey => "bugreport_help";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_configuration.BugReportLink))
        {
            context.ReplyRankAwareLocalizedMessage("bugreport_not_configured");
            return Task.CompletedTask;
        }

        context.ReplyRankAwareLocalizedMessage("bugreport_reply", _configuration.BugReportLink);
        return Task.CompletedTask;
    }
}
