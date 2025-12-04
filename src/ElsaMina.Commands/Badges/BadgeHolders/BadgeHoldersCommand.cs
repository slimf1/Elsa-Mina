using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.Badges.BadgeHolders;

[NamedCommand("badgeholders", "badge-holders")]
public class BadgeHoldersCommand : Command
{
    private readonly ITemplatesManager _templatesManager;
    private readonly IBotDbContextFactory _dbContextFactory;

    public BadgeHoldersCommand(ITemplatesManager templatesManager, IBotDbContextFactory dbContextFactory)
    {
        _templatesManager = templatesManager;
        _dbContextFactory = dbContextFactory;
    }

    public override Rank RequiredRank => Rank.Regular;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var badges = await dbContext.Badges
            .Include(badge => badge.BadgeHolders)
            .Where(badge => badge.RoomId == context.RoomId)
            .OrderBy(badge => badge.Name)
            .ToArrayAsync(cancellationToken);

        var viewModel = new BadgeHoldersViewModel
        {
            Culture = context.Culture,
            Badges = badges
        };
        var template = await _templatesManager.GetTemplateAsync("Badges/BadgeHolders/BadgeHolders", viewModel);
        context.ReplyHtml(template.RemoveNewlines(), rankAware: true);
    }
}