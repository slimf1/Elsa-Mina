using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Showdown.BattleTracker;

[NamedCommand("starttracking", "battletracking", "starttracking", "track")]
public class StartBattleTrackerCommand : Command
{
    
    
    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}