using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Services.UserDetails;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Profile;

public class ProfileCommand : ICommand
{
    public static string Name => "profile";
    public static IEnumerable<string> Aliases => new[] { "profil" };
    public bool IsAllowedInPm => true;
    public char RequiredRank => '+';

    private readonly IRepository<RoomSpecificUserData, Tuple<string, string>> _userDataRepository;
    private readonly IUserDetailsManager _userDetailsManager;
    private readonly ITemplatesManager _templatesManager;

    public ProfileCommand(IRepository<RoomSpecificUserData, Tuple<string, string>> userDataRepository,
        IUserDetailsManager userDetailsManager,
        ITemplatesManager templatesManager)
    {
        _userDataRepository = userDataRepository;
        _userDetailsManager = userDetailsManager;
        _templatesManager = templatesManager;
    }

    public async Task Run(IContext context)
    {
        var userId = string.IsNullOrEmpty(context.Target)
            ? context.Sender.UserId : context.Target.ToLowerAlphaNum();

        if (userId == null)
        {
            return;
        }

        var t1 = _userDataRepository.GetByIdAsync(new(userId, context.RoomId));
        var t2 = _userDetailsManager.GetUserDetails(userId);
        await Task.WhenAll(t1, t2);

        var storedUserData = t1.Result;
        var showdownUserDetails = t2.Result;

        var template = await _templatesManager.GetTemplate("profile", new Dictionary<string, object>
        {
            ["user_id"] = userId,
            ["user_name"] = showdownUserDetails?.Name ?? userId,
            ["avatar"] = showdownUserDetails?.Avatar
        });
        context.SendHtmlPage($"profile-{userId}", template.RemoveNewlines());
    }
}