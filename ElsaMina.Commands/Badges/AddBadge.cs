using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Badges;

public class AddBadge : ICommand
{
    public static string Name => "add-badge";

    public static IEnumerable<string> Aliases => new[]
        { "addbadge", "new-badge", "newbadge", "add-trophy", "newtrophy", "new-trophy" };

    public char RequiredRank => '%';

    private readonly IBadgeRepository _badgeRepository;

    public AddBadge(IBadgeRepository badgeRepository)
    {
        _badgeRepository = badgeRepository;
    }

    public async Task Run(IContext context)
    {
        var arguments = context.Target.Split(",");
        if (arguments.Length != 2)
        {
            // TODO : help message (i18n)
            context.Reply("Invalid syntax : -add-badge <name>, <image>");
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
            context.Reply("Added new badge ");
        }
        catch (Exception exception)
        {
            context.Reply($"An error occured : {exception.Message}");
        }
    }
}