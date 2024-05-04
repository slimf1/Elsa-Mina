using System.Globalization;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Services.UserData;
using ElsaMina.Core.Services.UserDetails;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Profile;

[NamedCommand("profile", Aliases = ["profil"])]
public class ProfileCommand : Command
{
    private const string DEFAULT_AVATAR_ID = "unknown";
    private const string AVATAR_URL = "https://play.pokemonshowdown.com/sprites/trainers/{0}.png";
    private const string AVATAR_CUSTOM_URL = "https://play.pokemonshowdown.com/sprites/trainers-custom/{0}.png";

    private readonly IRoomSpecificUserDataRepository _userDataRepository;
    private readonly IUserDetailsManager _userDetailsManager;
    private readonly ITemplatesManager _templatesManager;
    private readonly IRoomsManager _roomsManager;
    private readonly IConfigurationManager _configurationManager;
    private readonly IUserDataService _userDataService;

    public ProfileCommand(IRoomSpecificUserDataRepository userDataRepository,
        IUserDetailsManager userDetailsManager,
        ITemplatesManager templatesManager,
        IRoomsManager roomsManager,
        IConfigurationManager configurationManager,
        IUserDataService userDataService)
    {
        _userDataRepository = userDataRepository;
        _userDetailsManager = userDetailsManager;
        _templatesManager = templatesManager;
        _roomsManager = roomsManager;
        _configurationManager = configurationManager;
        _userDataService = userDataService;
    }
    
    public override bool IsAllowedInPm => true;
    public override char RequiredRank => '+';

    public override async Task Run(IContext context)
    {
        var userId = string.IsNullOrEmpty(context.Target)
            ? context.Sender.UserId : context.Target.ToLowerAlphaNum();

        if (userId == null)
        {
            return;
        }

        var t1 = _userDataRepository.GetByIdAsync(new Tuple<string, string>(userId, context.RoomId));
        var t2 = _userDetailsManager.GetUserDetails(userId);
        var t3 = _userDataService.GetRegisterDate(userId);
        await Task.WhenAll(t1, t2, t3);

        var storedUserData = t1.Result;
        var showdownUserDetails = t2.Result;
        var registerDate = t3.Result;

        var room = _roomsManager.GetRoom(context.RoomId);
        
        var status = GetStatus(showdownUserDetails);
        var avatarUrl = GetAvatar(storedUserData, showdownUserDetails);
        var userRoomRank = GetUserRoomRank(context, showdownUserDetails);

        var viewModel = new ProfileViewModel
        {
            Culture = new CultureInfo(room?.Locale ?? _configurationManager.Configuration.DefaultLocaleCode),
            Avatar = avatarUrl,
            UserId = userId,
            UserName = showdownUserDetails?.Name ?? userId,
            UserRoomRank = userRoomRank,
            Status = status,
            Badges = storedUserData?.Badges.Select(holding => holding.Badge),
            Title = storedUserData?.Title,
            RegisterDate = registerDate
        };
        var template = await _templatesManager.GetTemplate("Profile/Profile", viewModel);
        context.SendHtmlPage($"profile-{userId}", template.RemoveNewlines());
    }

    private static char GetUserRoomRank(IContext context, UserDetailsDto showdownUserDetails)
    {
        var userRoom = showdownUserDetails?
            .Rooms?
            .Keys
            .FirstOrDefault(roomName => roomName.ToLowerAlphaNum() == context.RoomId);
        return userRoom != null ? userRoom[0] : ' ';
    }

    private static string GetAvatar(RoomSpecificUserData storedUserData, UserDetailsDto showdownUserDetails)
    {
        string avatarUrl;
        if (!string.IsNullOrEmpty(storedUserData?.Avatar))
        {
            avatarUrl = storedUserData.Avatar;
        }
        else
        {
            var avatarId = showdownUserDetails?.Avatar ?? DEFAULT_AVATAR_ID;
            var avatarBaseUrl = AVATAR_URL;
            if (avatarId.StartsWith('#'))
            {
                avatarId = avatarId[1..];
                avatarBaseUrl = AVATAR_CUSTOM_URL;
            }
            avatarUrl = string.Format(avatarBaseUrl, avatarId);
        }

        return avatarUrl;
    }

    private static string GetStatus(UserDetailsDto showdownUserDetails)
    {
        var status = showdownUserDetails?.Status;
        if (status?.StartsWith('!') == true)
        {
            status = status[1..];
        }

        return status;
    }
}