using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Arcade.Events;

[NamedCommand("deleteeventrole")]
public class DeleteEventRoleCommand : Command
{
    private readonly IRoomsManager _roomsManager;
    private readonly IEventRoleMappingService _eventRoleMappingService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfiguration _configuration;

    public DeleteEventRoleCommand(IRoomsManager roomsManager,
        IEventRoleMappingService eventRoleMappingService,
        ITemplatesManager templatesManager,
        IConfiguration configuration)
    {
        _roomsManager = roomsManager;
        _eventRoleMappingService = eventRoleMappingService;
        _templatesManager = templatesManager;
        _configuration = configuration;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override bool IsAllowedInPrivateMessage => true;
    public override bool IsPrivateMessageOnly => true;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.Target))
        {
            return;
        }

        // Format: eventName;;roomId
        var parts = context.Target.Split(";;");
        var eventName = parts[0].Trim();
        var roomId = parts.Length > 1 ? parts[1].Trim() : context.RoomId;

        var room = _roomsManager.GetRoom(roomId);
        if (room == null)
        {
            context.ReplyLocalizedMessage("eventroles_room_not_found");
            return;
        }

        if (!await context.HasSufficientRankInRoom(roomId, Rank.Driver, cancellationToken))
        {
            return;
        }

        await _eventRoleMappingService.DeleteMappingAsync(eventName, roomId, cancellationToken);
        context.ReplyLocalizedMessage("eventroles_deleted", eventName);

        var mappings = await _eventRoleMappingService.GetMappingsForRoomAsync(roomId, cancellationToken);
        var viewModel = new EventRoleMappingViewModel
        {
            Culture = context.Culture,
            BotName = _configuration.Name,
            Trigger = _configuration.Trigger,
            RoomId = roomId,
            Mappings = mappings
        };
        var template = await _templatesManager.GetTemplateAsync("Arcade/EventRoleMappingDashboard", viewModel);
        context.ReplyHtmlPage($"{roomId}eventroles", template.RemoveNewlines());
    }
}
