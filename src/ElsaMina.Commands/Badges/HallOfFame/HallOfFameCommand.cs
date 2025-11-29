using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.Badges.HallOfFame;

[NamedCommand("halloffame", "hof", "hall-of-fame")]
public class HallOfFameCommand : Command
{
    private readonly ITemplatesManager _templatesManager;
    private readonly IBotDbContextFactory _dbContextFactory;

    public HallOfFameCommand(ITemplatesManager templatesManager, IBotDbContextFactory dbContextFactory)
    {
        _templatesManager = templatesManager;
        _dbContextFactory = dbContextFactory;
    }

    public override Rank RequiredRank => Rank.Regular;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var targetRoomId = string.IsNullOrWhiteSpace(context.Target)
            ? context.RoomId
            : context.Target.ToLowerAlphaNum();
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var trophies = await dbContext.Badges
            .Include(badge => badge.BadgeHolders)
            .Where(badge => badge.RoomId == targetRoomId && badge.IsTrophy)
            .ToArrayAsync(cancellationToken);

        if (trophies.Length == 0)
        {
            context.ReplyLocalizedMessage("hall_of_fame_no_trophies", targetRoomId);
            return;
        }

        var playerRecords = new Dictionary<string, PlayerRecord>();
        foreach (var trophy in trophies)
        {
            foreach (var holder in trophy.BadgeHolders)
            {
                if (!playerRecords.TryGetValue(holder.UserId, out var record))
                {
                    record = new PlayerRecord
                    {
                        UserId = holder.UserId,
                        Badges = [],
                        Total = 0,
                        Solo = 0,
                        Team = 0
                    };
                }

                record.Badges.Add(trophy);
                record.Total += 1;
                if (!trophy.IsTeamTournament)
                {
                    record.Solo += 1;
                }
                else
                {
                    record.Team += 1;
                }

                playerRecords[holder.UserId] = record;
            }
        }

        var template = await _templatesManager.GetTemplateAsync("Badges/HallOfFame/HallOfFame", new HallOfFameViewModel
        {
            Culture = context.Culture,
            SortedPlayerRecords = playerRecords.Values
                .OrderByDescending(record => record.Total)
                .ThenByDescending(record => record.Solo)
                .ThenByDescending(record => record.Team)
                .ToArray()
        });
        context.ReplyHtml(template.RemoveNewlines(), rankAware: true);
    }
}