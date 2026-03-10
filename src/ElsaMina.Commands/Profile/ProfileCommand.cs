using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.UserDetails;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Profile;

[NamedCommand("profile", Aliases = ["profil"])]
public class ProfileCommand : Command
{
    private readonly IProfileService _profileService;

    public ProfileCommand(IProfileService profileService)
    {
        _profileService = profileService;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override Rank RequiredRank => Rank.Regular;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var userId = string.IsNullOrEmpty(context.Target)
            ? context.Sender.UserId
            : context.Target.ToLowerAlphaNum();

        if (userId == null)
        {
            return;
        }

        var template = await _profileService.GetProfileHtmlAsync(userId, context.RoomId, cancellationToken);
        context.ReplyHtml(template.RemoveNewlines(), rankAware: true);
    }
}
