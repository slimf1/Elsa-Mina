using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Badges;

[NamedCommand("deletebadge", Aliases = ["deletetrophy", "delete-badge", "delete-trophy"])]
public class DeleteBadge : Command
{
    private readonly IBadgeRepository _badgeRepository;

    public DeleteBadge(IBadgeRepository badgeRepository)
    {
        _badgeRepository = badgeRepository;
    }

    public override Rank RequiredRank => Rank.Driver;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var badgeId = context.Target.ToLowerAlphaNum();
        var key = Tuple.Create(badgeId, context.RoomId);

        try
        {
            var badge = await _badgeRepository.GetByIdAsync(key, cancellationToken);
            if (badge == null)
            {
                context.ReplyLocalizedMessage("badge_delete_doesnt_exist", badgeId);
                return;
            }
            
            await _badgeRepository.DeleteAsync(badge, cancellationToken);
            await _badgeRepository.SaveChangesAsync(cancellationToken);
            context.ReplyLocalizedMessage("badge_delete_success", badgeId);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Error while deleting badge");
            context.ReplyLocalizedMessage("badge_delete_failure", exception.Message);
        }
    }
}