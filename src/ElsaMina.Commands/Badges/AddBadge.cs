using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Badges;

[NamedCommand("add-badge", Aliases = ["addbadge", "new-badge", "newbadge", "add-trophy", "newtrophy", "new-trophy"])]
public class AddBadge : Command
{
    private readonly IBotDbContextFactory _factory;

    public AddBadge(IBotDbContextFactory factory)
    {
        _factory = factory;
    }

    public override Rank RequiredRank => Rank.Driver;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
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

        await using var dbContext = await _factory.CreateDbContextAsync(cancellationToken);
        var existingBadge =
            await dbContext.Badges.FindAsync([badgeId, context.RoomId], cancellationToken: cancellationToken);
        if (existingBadge != null)
        {
            context.ReplyLocalizedMessage("badge_add_already_exist", name);
            return;
        }

        try
        {
            await dbContext.Badges.AddAsync(new Badge
            {
                Name = name,
                Image = image,
                Id = badgeId,
                IsTrophy = isTrophy,
                RoomId = context.RoomId
            }, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            context.ReplyLocalizedMessage("badge_add_success_message");
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Could not add badge");
            context.ReplyLocalizedMessage("badge_add_failure_message", exception.Message);
        }
    }
}