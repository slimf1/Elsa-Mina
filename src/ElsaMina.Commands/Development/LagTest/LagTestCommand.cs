using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Development.LagTest;

[NamedCommand("lagtest", Aliases = ["lag"])]
public class LagTestCommand : Command
{
    public override Rank RequiredRank => Rank.Regular;

    private readonly ILagTestManager _lagTestManager;

    public LagTestCommand(ILagTestManager lagTestManager)
    {
        _lagTestManager = lagTestManager;
    }

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var lagTestTask = _lagTestManager.StartLagTestAsync(context.RoomId, cancellationToken);
        context.Reply(LagTestManager.LAG_TEST_MARKER);

        var elapsed = await lagTestTask;

        if (elapsed == TimeSpan.MinValue)
        {
            context.ReplyRankAwareLocalizedMessage("lagtest_timeout");
            return;
        }

        context.ReplyRankAwareLocalizedMessage("lagtest_result", (long)elapsed.TotalMilliseconds);
    }
}
