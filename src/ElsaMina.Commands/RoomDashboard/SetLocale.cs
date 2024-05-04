using System.Globalization;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.RoomDashboard;

[NamedCommand("set-locale", Aliases = ["setlocale"])]
public class SetLocale : Command
{
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