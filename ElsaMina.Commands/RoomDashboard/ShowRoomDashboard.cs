using System.Globalization;
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
    private const string TABLE_STYLE = "border-collapse: collapse ; width: 100%";

    private const string FIRST_ROW_STYLE =
        "border: 1px solid #68a ; text-align: center ; padding: 0px ; height: 1px ; text-align: center ; background-color: rgba(102 , 136 , 170 , 0.45)";

    private const string CELL_STYLE = "border: 1px solid #68a ; text-align: center ; padding: 4px";

    private const string ROW_STYLE = "border: 1px solid #68a ; text-align: center ; padding: 4px";

    public static string Name => "room-dashboard";
    public bool IsPrivateMessageOnly => true;
    public bool IsWhitelistOnly => true; // todo : seul un mec authed sur la room peut

    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;
    private readonly IRoomsManager _roomsManager;
    private readonly IRoomParametersRepository _roomParametersRepository;

    public ShowRoomDashboard(IConfigurationManager configurationManager,
        IResourcesService resourcesService,
        IRoomsManager roomsManager,
        IRoomParametersRepository roomParametersRepository)
    {
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
        _roomsManager = roomsManager;
        _roomParametersRepository = roomParametersRepository;
    }

    public async Task Run(IContext context)
    {
        var roomId = context.Target.Trim().ToLower();
        if (string.IsNullOrEmpty(roomId))
        {
            roomId = context.RoomId;
        }

        if (!_roomsManager.HasRoom(roomId))
        {
            context.Reply(context.GetString("dashboard_room_doesnt_exist", roomId));
            return;
        }

        var panel = await GetPanel(context, roomId);
        context.SendHtmlPage($"{roomId}dashboard", panel.RemoveNewlines());
    }

    private async Task<string> GetPanel(IContext context, string roomId)
    {
        var roomParameters = await _roomParametersRepository.GetByIdAsync(roomId);
        if (roomParameters == null)
        {
            return "<p>Could not find room parameters somehow</p>";
        }

        if (context.IsPm)
        {
            context.Locale =
                new CultureInfo(roomParameters.Locale ?? _configurationManager.Configuration.DefaultLocaleCode);
        }
        var botName = _configurationManager.Configuration.Name;
        var localesOptions = _resourcesService.SupportedLocales.Select(culture => $"""
            <option
                value="{culture.Name}"
                {(context.Locale.Name == culture.Name ? "selected" : "")}>
                {culture.NativeName.Capitalize()}
            </option>
        """);

        return $$"""
            <form
                style="padding: 2rem;"
                id="locale"
                data-submitsend="/w {{botName}},-rc {{roomId}},{locale},{errors},{autocorrect}">
                <table style="{{TABLE_STYLE}}">
                    <thead>
                        <tr style="{{FIRST_ROW_STYLE}}">
                            <th colspan="3"><h1>{{context.GetString("room_dashboard", roomId)}}</h1></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr style="{{ROW_STYLE}}">
                            <td style="{{CELL_STYLE}}">{{context.GetString("dashboard_locale_name")}}</td>
                            <td style="{{CELL_STYLE}}">
                                <select name="locale">
                                    {{string.Join("", localesOptions)}}
                                </select>
                            </td>
                            <td style="{{CELL_STYLE}}">{{context.GetString("dashboard_locale_description")}}</td>
                        </tr>
                        <tr style="{{ROW_STYLE}}">
                            <td style="{{CELL_STYLE}}">{{context.GetString("dashboard_errors_name")}}</td>
                            <td style="{{CELL_STYLE}}">
                                <input
                                    type="checkbox"
                                    id="errors"
                                    name="errors"
                                    {{(roomParameters.IsShowingErrorMessages ?? false ? "checked" : "")}} />
                            </td>
                            <td style="{{CELL_STYLE}}">{{context.GetString("dashboard_errors_description")}}</td>
                        </tr>
                        <tr style="{{ROW_STYLE}}">
                            <td style="{{CELL_STYLE}}">{{context.GetString("dashboard_autocorrect_name")}}</td>
                            <td style="{{CELL_STYLE}}">
                                <input
                                    type="checkbox"
                                    id="autocorrect"
                                    name="autocorrect"
                                    {{(roomParameters.IsCommandAutocorrectEnabled ?? false ? "checked" : "")}} />
                            </td>
                            <td style="{{CELL_STYLE}}">{{context.GetString("dashboard_autocorrect_description")}}</td>
                        </tr>
                    </tbody>
                </table>
                <br /> <br />
                <button
                    class="button"
                    type="submit">
                    {{context.GetString("submit")}}
                </button>
            </form>
        """;
    }
}