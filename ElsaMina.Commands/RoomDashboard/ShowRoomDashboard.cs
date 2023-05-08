using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.RoomDashboard;

public class ShowRoomDashboard : ICommand
{
    public static string Name => "room-dashboard";
    public bool IsPrivateMessageOnly => true;
    public bool IsWhitelistOnly => true; // todo : seul un mec authed sur la room peut

    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;
    private readonly IRoomsManager _roomsManager;

    public ShowRoomDashboard(IConfigurationManager configurationManager,
        IResourcesService resourcesService,
        IRoomsManager roomsManager)
    {
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
        _roomsManager = roomsManager;
    }

    public Task Run(IContext context)
    {
        var roomId = context.Target.Trim().ToLower();
        if (string.IsNullOrEmpty(roomId))
        {
            roomId = context.RoomId;
        }

        if (!_roomsManager.HasRoom(roomId))
        {
            context.Reply(context.GetString("dashboard_room_doesnt_exist", roomId));
            return Task.CompletedTask;
        }

        context.SendHtmlPage($"{roomId}dashboard", GetPanel(context, roomId).RemoveNewlines());
        return Task.CompletedTask;
    }

    private string GetPanel(IContext context, string roomId)
    {
        var botName = _configurationManager.Configuration.Name;
        var localesOptions = _resourcesService.SupportedLocales.Select(culture =>
        {
            var localeDisplayName = string.IsNullOrEmpty(culture.Name)
                ? "Default (English)"
                : culture.NativeName.Capitalize();
            var localeName = string.IsNullOrEmpty(culture.Name) ? "en" : culture.Name;

            return $"""
            <option
                value="{localeName}"
                {(context.Locale.Name == localeName ? "selected" : "")}>
                {localeDisplayName}
            </option>
        """;
        });

         return $$"""
            <form
                style="padding: 2rem;"
                id="locale"
                data-submitsend="/w {{botName}},-rc {{roomId}}, {locale}">
                <h1>{{context.GetString("room_dashboard", roomId)}}</h1>
                <select name="locale">
                    {{string.Join("", localesOptions)}}
                </select>

                <br /> <br />
                <button
                    type="submit">
                    {{context.GetString("submit")}}
                </button>
            </form>
        """;
    }
}