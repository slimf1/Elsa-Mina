using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
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

    public ProfileCommand(IRepository<RoomSpecificUserData, Tuple<string, string>> userDataRepository)
    {
        _userDataRepository = userDataRepository;
    }

    public async Task Run(IContext context)
    {
        var userId = string.IsNullOrEmpty(context.Target)
            ? context.Sender.UserId : context.Target.ToLowerAlphaNum();
        var userData = await _userDataRepository.GetByIdAsync(new(userId, context.RoomId));
    }
}