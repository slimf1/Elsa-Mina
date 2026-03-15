using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("sharedpower")]
public class SharedPowerCommand : Command
{
    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "shared_power_help";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        context.Reply("/tour create randombattlemayhem,elim");
        context.Reply("/tour name [Gen 9] Random Battle Shared Power");
        context.Reply("/tour rules !scalemonsmod,!camomonsmod,!inversemod");
        context.Reply("/wall Tournoi en Shared Power !");
        context.Reply("!rfaq sharedpower");
        return Task.CompletedTask;
    }
}
