﻿using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using Serilog;

namespace ElsaMina.Commands.Badges;

public class DeleteBadge : ICommand
{
    private readonly IRepository<Badge, Tuple<string, string>> _badgeRepository;
    private readonly ILogger _logger;

    public DeleteBadge(IRepository<Badge, Tuple<string, string>> badgeRepository, ILogger logger)
    {
        _badgeRepository = badgeRepository;
        _logger = logger;
    }

    public static string Name => "deletebadge";
    public static IEnumerable<string> Aliases => new[] { "deletetrophy", "delete-badge", "delete-trophy" };
    public char RequiredRank => '%';

    public async Task Run(IContext context)
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
            _logger.Error(exception, "Error while deleting badge");
            context.ReplyLocalizedMessage("badge_delete_failure", exception.Message);
        }
    }
}