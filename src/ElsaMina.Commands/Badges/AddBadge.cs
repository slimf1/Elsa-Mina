using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Badges;

public class AddBadge : Command<AddBadge>, INamed
{
    public static string Name => "add-badge";

    public static List<string> Aliases =>
        ["addbadge", "new-badge", "newbadge", "add-trophy", "newtrophy", "new-trophy"];

    private readonly IBadgeRepository _badgeRepository;

    public AddBadge(IBadgeRepository badgeRepository)
    {
        _badgeRepository = badgeRepository;
    }

    public override char RequiredRank => '%';

    public override async Task Run(IContext context)
    {
        var arguments = context.Target.Split(",");
        if (arguments.Length != 2)
        {
            context.ReplyLocalizedMessage("badge_help_message");
            return;
        }

        var name = arguments[0].Trim();
        var image = arguments[1].Trim();
        var isTrophy = context.Command is "add-trophy" or "newtrophy" or "new-trophy";
        var badgeId = name.ToLowerAlphaNum();

        var existingBadge = await _badgeRepository.GetByIdAsync(new Tuple<string, string>(badgeId, context.RoomId));
        if (existingBadge != null)
        {
            context.ReplyLocalizedMessage("badge_add_already_exist", name);
            return;
        }

        try
        {
            await _badgeRepository.AddAsync(new Badge
            {
                Name = name,
                Image = image,
                Id = badgeId,
                IsTrophy = isTrophy,
                RoomId = context.RoomId
            });
            context.ReplyLocalizedMessage("badge_add_success_message");
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "Could not add badge");
            context.ReplyLocalizedMessage("badge_add_failure_message", exception.Message);
        }
    }
}