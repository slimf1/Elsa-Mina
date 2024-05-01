using System.Globalization;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.RoomDashboard;

public class SetLocale : Command<SetLocale>, INamed
{
    public static string Name => "set-locale";
    public static IEnumerable<string> Aliases => new[] { "setlocale" };

    private readonly IRoomParametersRepository _roomParametersRepository;

    public SetLocale(IRoomParametersRepository roomParametersRepository)
    {
        _roomParametersRepository = roomParametersRepository;
    }

    public override char RequiredRank => '#';

    public override async Task Run(IContext context)
    {
        var arguments = context.Target.Split(",");
        var roomId = arguments[0].Trim();
        var locale = arguments[1].Trim();
        CultureInfo cultureInfo;
        try
        {
            cultureInfo = new CultureInfo(locale);
        }
        catch (CultureNotFoundException)
        {
            context.Reply($"Locale '{locale}' doesn't exist.");
            return;
        }

        var roomParameters = await _roomParametersRepository.GetByIdAsync(roomId);
        if (roomParameters == null)
        {
            context.Reply($"Room '{roomId}' not found.");
            return;
        }
        roomParameters.Locale = locale;
        await _roomParametersRepository.UpdateAsync(roomParameters);
        context.Culture = cultureInfo;
        context.Reply($"Updated locale of room {roomId} to : {locale}");
    }
}