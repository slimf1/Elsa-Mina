using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Badges;

[NamedCommand("givebadge", Aliases = ["give-badge"])]
public class GiveBadge : Command
{
    private readonly IBadgeRepository _badgeRepository;
    private readonly IRoomUserDataService _roomUserDataService;

    public GiveBadge(IBadgeRepository badgeRepository,
        IRoomUserDataService roomUserDataService)
    {
        _badgeRepository = badgeRepository;
        _roomUserDataService = roomUserDataService;
    }
    
    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "badge_give_help_message";

    public override async Task Run(IContext context)
    {
        var parts = context.Target.Split(",");
        if (parts.Length != 2)
        {
            ReplyLocalizedHelpMessage(context);
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
            Log.Error(exception, "An error occurred while fetching a badge");
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
            Log.Error(exception, "An error occurred while giving a badge");
            context.ReplyLocalizedMessage("badge_give_error", exception.Message);
        }
    }
}