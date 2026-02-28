using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.Logging;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.Users.PlayTimes;

[NamedCommand("topplaytimes", Aliases = ["tpt", "toptimes"])]
public class TopPlayTimesCommand : Command
{
    private const int TOP_COUNT = 20;

    private readonly IBotDbContextFactory _botDbContextFactory;
    private readonly ITemplatesManager _templatesManager;
    private readonly IRoomsManager _roomsManager;

    public TopPlayTimesCommand(IBotDbContextFactory botDbContextFactory, ITemplatesManager templatesManager,
        IRoomsManager roomsManager)
    {
        _botDbContextFactory = botDbContextFactory;
        _templatesManager = templatesManager;
        _roomsManager = roomsManager;
    }

    public override Rank RequiredRank => Rank.Regular;
    public override string HelpMessageKey => "top_play_times_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var roomId = string.IsNullOrEmpty(context.Target)
                ? context.RoomId
                : context.Target.ToLowerAlphaNum();

            await using var dbContext = await _botDbContextFactory.CreateDbContextAsync(cancellationToken);
            var topUsers = await dbContext.RoomUsers
                .Where(roomUser => roomUser.RoomId == roomId && roomUser.PlayTime > TimeSpan.Zero)
                .Include(roomUser => roomUser.User)
                .OrderByDescending(roomUser => roomUser.PlayTime)
                .Take(TOP_COUNT)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (topUsers.Count == 0)
            {
                context.ReplyLocalizedMessage("top_play_times_no_data");
                return;
            }

            var topList = topUsers
                .Select((roomUser, i) => new TopPlayTimesEntry(
                    Rank: i + 1,
                    UserId: roomUser.Id,
                    UserName: roomUser.User?.UserName ?? roomUser.Id,
                    PlayTime: roomUser.PlayTime))
                .ToList();

            var roomLabel = _roomsManager.GetRoom(roomId)?.Name ?? roomId;
            var template = await _templatesManager.GetTemplateAsync("Users/PlayTimes/TopPlayTimesTable",
                new TopPlayTimesViewModel
                {
                    Culture = context.Culture,
                    Room = roomLabel,
                    TopList = topList
                });

            context.ReplyHtml(template.RemoveNewlines().RemoveWhitespacesBetweenTags(), rankAware: true);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Failed to get top play times");
            context.ReplyLocalizedMessage("top_play_times_error");
        }
    }
}