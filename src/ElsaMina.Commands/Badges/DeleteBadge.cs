﻿using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Badges;

[NamedCommand("deletebadge", Aliases = ["deletetrophy", "delete-badge", "delete-trophy"])]
public class DeleteBadge : Command
{
    private readonly IBadgeRepository _badgeRepository;

    public DeleteBadge(IBadgeRepository badgeRepository)
    {
        _badgeRepository = badgeRepository;
    }

    public override char RequiredRank => '%';

    public override async Task Run(IContext context)
    {
        var badgeId = context.Target.ToLowerAlphaNum();
        var key = new Tuple<string, string>(badgeId, context.RoomId);
        if (await _badgeRepository.GetByIdAsync(key) == null)
        {
            context.ReplyLocalizedMessage("badge_delete_doesnt_exist", badgeId);
            return;
        }

        try
        {
            await _badgeRepository.DeleteAsync(key);
            context.ReplyLocalizedMessage("badge_delete_success", badgeId);
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "Error while deleting badge");
            context.ReplyLocalizedMessage("badge_delete_failure", exception.Message);
        }
    }
}