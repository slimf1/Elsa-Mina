using ElsaMina.Commands.Showdown.Ranking;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Services.UserData;
using ElsaMina.Core.Services.UserDetails;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Profile;

[NamedCommand("profile", Aliases = ["profil"])]
public class ProfileCommand : Command
{
    private const string DEFAULT_AVATAR_ID = "167";
    private const string AVATAR_URL = "https://play.pokemonshowdown.com/sprites/trainers/{0}.png";
    private const string AVATAR_CUSTOM_URL = "https://play.pokemonshowdown.com/sprites/trainers-custom/{0}.png";

    private readonly IRoomSpecificUserDataRepository _userDataRepository;
    private readonly IUserDetailsManager _userDetailsManager;
    private readonly ITemplatesManager _templatesManager;
    private readonly IUserDataService _userDataService;
    private readonly IShowdownRanksProvider _showdownRanksProvider;
    private readonly IFormatsManager _formatsManager;

    public ProfileCommand(IRoomSpecificUserDataRepository userDataRepository,
        IUserDetailsManager userDetailsManager,
        ITemplatesManager templatesManager,
        IUserDataService userDataService,
        IShowdownRanksProvider showdownRanksProvider,
        IFormatsManager formatsManager)
    {
        _userDataRepository = userDataRepository;
        _userDetailsManager = userDetailsManager;
        _templatesManager = templatesManager;
        _userDataService = userDataService;
        _showdownRanksProvider = showdownRanksProvider;
        _formatsManager = formatsManager;
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

        var userDataTask = _userDataRepository.GetByIdAsync(Tuple.Create(userId, context.RoomId), cancellationToken);
        var userDetailsTask = _userDetailsManager.GetUserDetailsAsync(userId, cancellationToken);
        var registerDateTask = _userDataService.GetRegisterDateAsync(userId, cancellationToken);
        var ranksTask = _showdownRanksProvider.GetRankingDataAsync(userId, cancellationToken);
        await Task.WhenAll(userDataTask, userDetailsTask, registerDateTask, ranksTask);

        var storedUserData = userDataTask.Result;
        var showdownUserDetails = userDetailsTask.Result;
        var registerDate = registerDateTask.Result;

        var room = context.Room;
        var status = GetStatus(showdownUserDetails);
        var avatarUrl = GetAvatar(storedUserData, showdownUserDetails);
        var userRoomRank = GetUserRoomRank(context, showdownUserDetails);
        var bestRanking = ranksTask.Result?.MaxBy(ranking => ranking.Elo);

        if (bestRanking != null)
        {
            bestRanking.FormatId = _formatsManager.GetCleanFormat(bestRanking.FormatId);
        }

        var viewModel = new ProfileViewModel
        {
            Culture = room.Culture,
            Avatar = avatarUrl,
            UserId = userId,
            UserName = showdownUserDetails?.Name ?? userId,
            UserRoomRank = userRoomRank,
            Status = status,
            Badges = storedUserData?.Badges.Select(holding => holding.Badge),
            Title = storedUserData?.Title,
            RegisterDate = registerDate,
            BestRanking = bestRanking
        };
        var template = await _templatesManager.GetTemplateAsync("Profile/Profile", viewModel);
        context.ReplyHtml(template.RemoveNewlines(), rankAware: true);
    }

    private static char GetUserRoomRank(IContext context, UserDetailsDto showdownUserDetails)
    {
        var userRoom = showdownUserDetails?
            .Rooms?
            .Keys
            .FirstOrDefault(roomName => roomName.ToLowerAlphaNum() == context.RoomId);
        return userRoom != null ? userRoom[0] : ' ';
    }

    public static string GetAvatar(RoomSpecificUserData storedUserData, UserDetailsDto showdownUserDetails)
    {
        string avatarUrl;
        if (!string.IsNullOrEmpty(storedUserData?.Avatar))
        {
            avatarUrl = storedUserData.Avatar;
        }
        else
        {
            var avatarId = showdownUserDetails?.Avatar ?? DEFAULT_AVATAR_ID;
            if (BattleAvatars.AVATAR_NUMBERS.TryGetValue(avatarId, out var avatarName))
            {
                avatarId = avatarName;
            }

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