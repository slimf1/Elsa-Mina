using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using Serilog;

namespace ElsaMina.Commands.Badges;

public class AddBadge : ICommand
{
    public static string Name => "add-badge";

    public static IEnumerable<string> Aliases => new[]
        { "addbadge", "new-badge", "newbadge", "add-trophy", "newtrophy", "new-trophy" };

    public char RequiredRank => '%';

    private readonly ILogger _logger;
    private readonly IRepository<Badge, Tuple<string, string>> _badgeRepository;

    public AddBadge(ILogger logger, IRepository<Badge, Tuple<string, string>> badgeRepository)
    {
        _logger = logger;
        _badgeRepository = badgeRepository;
    }

    public async Task Run(IContext context)
    {
        var arguments = context.Target.Split(",");
        if (arguments.Length != 2)
        {
            context.Reply(context.GetString("badge_help_message"));
            return;
        }

        var name = arguments[0].Trim();
        var image = arguments[1].Trim();
        var isTrophy = context.Command is "add-trophy" or "newtrophy" or "new-trophy";

        try
        {
            await _badgeRepository.AddAsync(new Badge
            {
                Name = name,
                Image = image,
                Id = name.ToLowerAlphaNum(),
                IsTrophy = isTrophy
            });
            context.Reply(context.GetString("badge_add_success_message"));
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "Could not add badge");
            context.Reply(context.GetString("badge_add_failure_message", exception.Message));
        }
    }
}