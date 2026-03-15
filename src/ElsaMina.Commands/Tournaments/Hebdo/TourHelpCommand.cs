using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("tourhelp")]
public class TourHelpCommand : Command
{
    private static readonly string[] TOUR_COMMANDS =
    [
        "sharedpower", "hebdosv", "hebdoss", "hebdosm", "hebdoaaa",
        "hebdobh", "hebdomnm", "hebdogg", "hebdostab", "hebdopic",
        "hebdoinhe", "hebdocamo", "hebdonfe", "hebdo1v1", "hebdoag",
        "hebdolcuu", "hebdoubersuu", "hebdozu", "hebdoadvru", "hebdobwru",
        "hebdoorasru", "hebdosmru", "hebdossru"
    ];

    public override Rank RequiredRank => Rank.Driver;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var commandList = string.Join("<br>", TOUR_COMMANDS.Select(name => $"- {name}"));
        context.ReplyHtml($"{context.GetString("tour_help_title")}<br>{commandList}");
        return Task.CompletedTask;
    }
}
