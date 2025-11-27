using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.Badges.BadgeDisplay;

[NamedCommand("badge", "showbadge", "show-badge")]
public class BadgeDisplayCommand : Command
{
    private readonly IBotDbContextFactory _dbContextFactory;
    private readonly ITemplatesManager _templatesManager;

    public BadgeDisplayCommand(IBotDbContextFactory dbContextFactory, ITemplatesManager templatesManager)
    {
        _dbContextFactory = dbContextFactory;
        _templatesManager = templatesManager;
    }

    public override Rank RequiredRank => Rank.Regular;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var badgeId = context.Target.ToLowerAlphaNum();
        var badge = await dbContext.Badges
            .Include(badge => badge.BadgeHolders)
            .Where(badge => badge.RoomId == context.RoomId && badge.Id == badgeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (badge == null)
        {
            context.ReplyLocalizedMessage("badge_display_not_found", context.Target);
            return;
        }

        var viewModel = new BadgeDisplayViewModel
        {
            DisplayedBadge = badge,
            BadgeHolders = badge.BadgeHolders.Select(holding => holding.UserId).ToArray(),
            Culture = context.Culture
        };
        var template = await _templatesManager.GetTemplateAsync("Badges/BadgeDisplay/BadgeDisplay", viewModel);
        context.ReplyHtml(template.RemoveNewlines(), rankAware: true);
    }
}