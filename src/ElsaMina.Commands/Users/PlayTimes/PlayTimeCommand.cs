using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.Logging;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.Users.PlayTimes;

[NamedCommand("playtime", Aliases = ["pt", "time"])]
public class PlayTimeCommand : Command
{
    private readonly IBotDbContextFactory _botDbContextFactory;

    public PlayTimeCommand(IBotDbContextFactory botDbContextFactory)
    {
        _botDbContextFactory = botDbContextFactory;
    }

    public override Rank RequiredRank => Rank.Regular;
    public override string HelpMessageKey => "play_time_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = string.IsNullOrWhiteSpace(context.Target)
                ? context.Sender.UserId
                : context.Target.ToLowerAlphaNum();

            await using var dbContext = await _botDbContextFactory.CreateDbContextAsync(cancellationToken);
            var roomUser = await dbContext.RoomUsers
                .Where(roomUser => roomUser.Id == userId && roomUser.RoomId == context.RoomId)
                .Include(roomUser => roomUser.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (roomUser == null)
            {
                context.ReplyLocalizedMessage("play_time_no_data", userId);
                return;
            }

            var displayName = roomUser.User?.UserName ?? roomUser.Id;
            var formattedTime = roomUser.PlayTime.ToPlayTimeString(context.GetString("play_time_format"));
            context.ReplyLocalizedMessage("play_time_result", displayName, formattedTime, context.RoomId);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Failed to get play time");
            context.ReplyLocalizedMessage("play_time_error");
        }
    }
}
