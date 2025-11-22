using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Badges;

[NamedCommand("hall-of-fame", "hof", "halloffame")]
public class HallOfFameCommand : Command
{
    public override Rank RequiredRank => Rank.Regular;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}