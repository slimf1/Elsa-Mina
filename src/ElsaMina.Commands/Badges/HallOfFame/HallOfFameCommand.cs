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
        // 1. Get Room ID (handles optional target room argument)
        string targetRoomId = context.Target.IsNullOrWhiteSpace() 
            ? context.RoomId 
            : context.Target.ToId();

        // 2. Room existence check (optional, but good practice)
        if (!context.Bot.Rooms.ContainsKey(targetRoomId))
        {
            await context.ReplyAsync($"Could not find the \"{targetRoomId}\" room", cancellationToken);
            return;
        }

        // 3. Database query: Get all trophies for the room
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Fetch all badges that are marked as trophies (Badge.IsTrophy corresponds to badge[3] in Python)
        // and include their holders
        var trophies = await dbContext.Badges
            .Include(badge => badge.BadgeHolders)
            .Where(badge => badge.RoomId == targetRoomId && badge.IsTrophy)
            .ToArrayAsync(cancellationToken);

        // 4. Hall of Fame Logic
        var users = new Dictionary<string, HallOfFameUserStats>();
        
        // This is the hardcoded list from the Python code
        var teamBadgeIds = new[] { 
            "bttbr", "pokelandtriforce", "pokelandpremierleague", "pokelandteamtournament", 
            "pokelanddoubletournament", "extraligue", "bigbangsuperleague", "bbpl", 
            "bbrt", "bbsl", "tournoidesrgions", "frenchcommunityleague", 
            "frenchcommunitypremierleague", "bigbangpremierleague", "bigbangregionaltournament", 
            "plpliv", "pltbr", "plttv", "burnedtowerteambattleroyal", 
            "burnedtowerteambattleroyalii", "burnedtowerpremierleague", 
            "burnedtowerteambatteroyalspecialedition", "pokelandpremierleaguev", 
            "burnedtowerelitechampionship", "pokelandteambattleroyalii" 
        };

        foreach (var badge in trophies)
        {
            // Python's is_team logic: check if any team ID is contained in the badge ID
            bool isTeam = teamBadgeIds.Any(teamId => badge.Id.Contains(teamId));

            // The Python code retrieves the username from the Room class, but since the C#
            // command doesn't use the Room object to get the badge holders, we rely on the
            // DB/User class later to get the display name for the ranking.
            foreach (var holder in badge.BadgeHolders)
            {
                if (!users.TryGetValue(holder.UserId, out var stats))
                {
                    stats = new HallOfFameUserStats { UserId = holder.UserId };
                    users.Add(holder.UserId, stats);
                }

                stats.Count++;
                stats.ImagesHtml.Add(BadgeUtils.GetBadgeImgHtml(badge)); // Helper needed for HTML
                
                if (isTeam)
                {
                    stats.TeamCount++;
                }
                else
                {
                    stats.SoloCount++;
                }
            }
        }

        // 5. Sorting (same key as Python: Count DESC, then SoloCount DESC)
        var sortedUsers = users.Values
            .OrderByDescending(u => u.Count)
            .ThenByDescending(u => u.SoloCount)
            .ToList();

        // 6. Template preparation and reply
        var viewModel = new HallOfFameViewModel
        {
            Culture = context.Culture,
            RoomName = context.Bot.Rooms[targetRoomId].Name, // Get the display name of the room
            RankedUsers = sortedUsers
        };
        var template = await _templatesManager.GetTemplateAsync("Badges/HallOfFame/HallOfFame", viewModel);
        context.ReplyHtml(template.RemoveNewlines(), rankAware: true);
    }
}