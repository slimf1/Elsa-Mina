using System.Globalization;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.RoomDashboard;

[NamedCommand("set-locale", Aliases = ["setlocale"])]
public class SetLocale : Command
{
    private readonly IRoomsManager _roomsManager;

    public SetLocale(IRoomsManager roomsManager)
    {
        _roomsManager = roomsManager;
    }

    public override Rank RequiredRank => Rank.RoomOwner;

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

        var success = await _roomsManager.SetRoomBotConfigurationParameterValue(roomId,
            RoomParametersConstants.LOCALE, locale);
        context.Culture = cultureInfo;
        context.Reply(success ? $"Updated locale of room {roomId} to : {locale}" : "An error occurred.");
    }
}