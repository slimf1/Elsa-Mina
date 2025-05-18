using System.Text;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.RoomDashboard;

[NamedCommand("room-dashboard")]
public class ShowRoomDashboard : Command
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IRoomsManager _roomsManager;
    private readonly ITemplatesManager _templatesManager;

    public ShowRoomDashboard(IConfigurationManager configurationManager,
        IRoomsManager roomsManager,
        ITemplatesManager templatesManager)
    {
        _configurationManager = configurationManager;
        _roomsManager = roomsManager;
        _templatesManager = templatesManager;
    }

    public override bool IsPrivateMessageOnly => true;
    public override bool IsWhitelistOnly => true; // todo : seul un mec authed sur la room peut

    // TODO : à revoir
    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var roomId = context.Target.Trim().ToLower();
        if (string.IsNullOrEmpty(roomId))
        {
            roomId = context.RoomId;
        }

        var room = _roomsManager.GetRoom(roomId);

        if (room == null)
        {
            context.ReplyLocalizedMessage("dashboard_room_doesnt_exist", roomId);
            return;
        }

        if (context.IsPrivateMessage)
        {
            context.Culture = room.Culture;
        }

        var configurationCommandBuilder = new StringBuilder("/w ");
        configurationCommandBuilder.Append(_configurationManager.Configuration.Name);
        configurationCommandBuilder.Append(',');
        configurationCommandBuilder.Append(_configurationManager.Configuration.Trigger);
        configurationCommandBuilder.Append("rc ");
        configurationCommandBuilder.Append(roomId);
        configurationCommandBuilder.Append(',');
        configurationCommandBuilder.AppendJoin(',', _roomsManager
            .RoomParameters
            .Values
            .Select(parameter => $"{parameter.Identifier}={{{parameter.Identifier}}}"));

        var viewModel = new RoomDashboardViewModel
        {
            BotName = _configurationManager.Configuration.Name,
            Trigger = _configurationManager.Configuration.Trigger,
            RoomId = roomId,
            Command = configurationCommandBuilder.ToString(),
            RoomName = room.Name,
            Culture = context.Culture,
            RoomParameterLines = _roomsManager.RoomParameters
                .Select(roomParameter => new RoomParameterLineModel
                {
                    Culture = context.Culture,
                    RoomParameter = roomParameter.Value,
                    CurrentValue = _roomsManager
                        .GetRoomParameter(roomId, roomParameter.Value.Identifier)
                })
        };
        var template = await _templatesManager.GetTemplateAsync("RoomDashboard/RoomDashboard", viewModel);

        context.ReplyHtmlPage($"{roomId}dashboard", template.RemoveNewlines());
    }
}