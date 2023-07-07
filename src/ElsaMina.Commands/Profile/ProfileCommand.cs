using System.Globalization;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Services.UserData;
using ElsaMina.Core.Services.UserDetails;
using ElsaMina.Core.Templates.Profile;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Profile;

public class ProfileCommand : BaseCommand<ProfileCommand>, INamed
{
    private const string DEFAULT_AVATAR_ID = "unknown";
    private const string AVATAR_URL = "https://play.pokemonshowdown.com/sprites/trainers/{0}.png";
    private const string AVATAR_CUSTOM_URL = "https://play.pokemonshowdown.com/sprites/trainers-custom/{0}.png";
    
    public static string Name => "profile";
    public static IEnumerable<string> Aliases => new[] { "profil" };

    private readonly IRepository<RoomSpecificUserData, Tuple<string, string>> _userDataRepository;
    private readonly IUserDetailsManager _userDetailsManager;
    private readonly ITemplatesManager _templatesManager;
    private readonly IRoomsManager _roomsManager;
    private readonly IConfigurationManager _configurationManager;
    private readonly IUserDataService _userDataService;

    public ProfileCommand(IRepository<RoomSpecificUserData, Tuple<string, string>> userDataRepository,
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

        var t1 = _userDataRepository.GetByIdAsync(new(userId, context.RoomId));
        var t2 = _userDetailsManager.GetUserDetails(userId);
        var t3 = _userDataService.GetRegisterDate(userId);
        await Task.WhenAll(t1, t2, t3);

        var storedUserData = t1.Result;
        var showdownUserDetails = t2.Result;
        var registerDate = t3.Result;

        var room = _roomsManager.GetRoom(context.RoomId);
        
        // Status
        var status = showdownUserDetails?.Status;
        if (status?.StartsWith("!") == true)
        {
            status = status[1..];
        }
        
        // Avatar
        string avatarUrl;
        if (!string.IsNullOrEmpty(storedUserData?.Avatar))
        {
            avatarUrl = storedUserData.Avatar;
        }
        else
        {
            var avatarId = showdownUserDetails?.Avatar ?? DEFAULT_AVATAR_ID;
            var avatarBaseUrl = AVATAR_URL;
            if (avatarId.StartsWith("#"))
            {
                avatarId = avatarId[1..];
                avatarBaseUrl = AVATAR_CUSTOM_URL;
            }
            avatarUrl = string.Format(avatarBaseUrl, avatarId);
        }
        
        // Rank
        var userRoom = showdownUserDetails?
            .Rooms?
            .Keys
            .FirstOrDefault(roomName => roomName.ToLowerAlphaNum() == context.RoomId);

        var viewModel = new ProfileViewModel
        {
            Culture = new CultureInfo(room?.Locale ?? _configurationManager.Configuration.DefaultLocaleCode),
            Avatar = avatarUrl,
            UserId = userId,
            UserName = showdownUserDetails?.Name ?? userId,
            UserRoomRank = userRoom != null ? userRoom[0] : ' ',
            Status = status,
            Badges = storedUserData?.Badges.Select(holding => holding.Badge),
            Title = storedUserData?.Title,
            RegisterDate = registerDate
        };
        var template = await _templatesManager.GetTemplate("Profile/Profile", viewModel);
        context.SendHtmlPage($"profile-{userId}", template.RemoveNewlines());
    }
}