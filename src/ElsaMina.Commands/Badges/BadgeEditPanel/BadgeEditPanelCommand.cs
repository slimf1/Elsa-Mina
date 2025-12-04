using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;

namespace ElsaMina.Commands.Badges.BadgeEditPanel;

[NamedCommand("badge-edit", "editbadge", "updatebadge", "editbadge", "updatebadges")]
public class BadgeEditPanelCommand : Command
{
    private readonly IBotDbContextFactory _dbContextFactory;

    public BadgeEditPanelCommand(IBotDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override Rank RequiredRank => Rank.Regular;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}