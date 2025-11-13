using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Badges;

[NamedCommand("badge-edit", "editbadge", "updatebadge", "editbadge", "updatebadges")]
public class BadgeEditPanelCommand : Command
{
    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}