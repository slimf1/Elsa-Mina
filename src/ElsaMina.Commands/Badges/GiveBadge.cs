using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using Serilog;

namespace ElsaMina.Commands.Badges;

public class GiveBadge : ICommand
{
    public static string Name => "givebadge";
    public static IEnumerable<string> Aliases => new[] { "give-badge" };
    public char RequiredRank => '%';
    public string HelpMessageKey => "badge_give_help_message";

    private readonly ILogger _logger;
    private readonly IRepository<Badge, Tuple<string, string>> _badgeRepository;
    private readonly IRoomUserDataService _roomUserDataService;

    public GiveBadge(ILogger logger,
        IRepository<Badge, Tuple<string, string>> badgeRepository,
        IRoomUserDataService roomUserDataService)
    {
        _logger = logger;
        _badgeRepository = badgeRepository;
        _roomUserDataService = roomUserDataService;
    }

    public async Task Run(IContext context)
    {
        var parts = context.Target.Split(",");
        if (parts.Length != 2)
        {
            context.ReplyLocalizedMessage(HelpMessageKey);
            return;
        }
        var userId = parts[0].ToLowerAlphaNum();
        var badgeId = parts[1].ToLowerAlphaNum();

        Badge badge = null;
        try
        {
            badge = await _badgeRepository.GetByIdAsync(new(badgeId, context.RoomId));
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "An error occured while fetching a badge");
        }

        if (badge == null)
        {
            context.ReplyLocalizedMessage("badge_give_could_not_find_badge", badgeId);
            return;
        }

        try
        {
            await _roomUserDataService.GiveBadgeToUser(context.RoomId, userId, badgeId);
            context.ReplyLocalizedMessage("badge_give_success", userId, badge.Name);
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "An error occured while giving a badge");
            context.ReplyLocalizedMessage("badge_give_error", exception.Message);
        }
    }
}