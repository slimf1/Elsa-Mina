using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;
using Serilog;

namespace ElsaMina.Commands.Badges;

public class TakeBadge : Command<TakeBadge>, INamed
{
    public static string Name => "takebadge";
    public static IEnumerable<string> Aliases => new[] { "take-badge", "take-trophy", "taketrophy" };

    private readonly ILogger _logger;
    private readonly IRoomUserDataService _roomUserDataService;

    public TakeBadge(ILogger logger,
        IRoomUserDataService roomUserDataService)
    {
        _logger = logger;
        _roomUserDataService = roomUserDataService;
    }

    public override char RequiredRank => '%';
    public override string HelpMessageKey => "takebadge_help_message";
    
    public override async Task Run(IContext context)
    {
        var parts = context.Target.Split(",");
        if (parts.Length != 2)
        {
            context.ReplyLocalizedMessage(HelpMessageKey);
            return;
        }

        var userId = parts[0].ToLowerAlphaNum();
        var badgeId = parts[1].ToLowerAlphaNum();

        try
        {
            await _roomUserDataService.TakeBadgeFromUser(context.RoomId, userId, badgeId);
            context.ReplyLocalizedMessage("takebadge_success", userId, badgeId);
        }
        catch (ArgumentException)
        {
            context.ReplyLocalizedMessage("takebadge_badge_doesnt_exist", userId, badgeId);
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "An error occcurred while taking badge");
            context.ReplyLocalizedMessage("takebadge_failure", exception.Message);
        }
    }
}