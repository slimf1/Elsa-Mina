using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Tournaments.Rpplf;

public abstract class RpplfTournamentCommand : Command
{
    protected abstract string Format { get; }
    protected abstract string TourName { get; }
    protected abstract string TeamsName { get; }
    protected abstract string TourRules { get; }
    protected virtual int? AutoDq => null;

    public override Rank RequiredRank => Rank.Driver;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        context.Reply($"/tour create {Format}, elim");
        context.Reply("/tour autostart 10");

        if (AutoDq.HasValue)
        {
            context.Reply($"/tour autodq {AutoDq.Value}");
        }

        context.Reply($"/tour name {TourName}");
        context.Reply($"-teams {TeamsName}");
        context.Reply($"/tour rules {TourRules}");

        return Task.CompletedTask;
    }
}
