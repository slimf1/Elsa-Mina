using System.Globalization;
using ElsaMina.Core;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Rooms.Parameters;

namespace ElsaMina.Commands.Teams.Tournaments;

public class OtherRoomTournamentAnnounceHandler : Handler
{
    private readonly IConfiguration _configuration;
    private readonly IBot _bot;
    private readonly IFormatsManager _formatsManager;
    private readonly IResourcesService _resourcesService;
    private readonly IRoomsManager _roomsManager;

    public OtherRoomTournamentAnnounceHandler(IConfiguration configuration, IBot bot, IFormatsManager formatsManager,
        IResourcesService resourcesService, IRoomsManager roomsManager)
    {
        _configuration = configuration;
        _bot = bot;
        _formatsManager = formatsManager;
        _resourcesService = resourcesService;
        _roomsManager = roomsManager;
    }

    public override Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default)
    {
        if (parts.Length < 4 || parts[1] != "tournament" || parts[2] != "create")
        {
            return Task.CompletedTask;
        }

        var format = _formatsManager.GetCleanFormat(parts[3]);
        foreach (var (broadcastingRoomId, receivingRoomsIds) in _configuration.TourAnnounces)
        {
            if (roomId != broadcastingRoomId)
            {
                continue;
            }

            foreach (var receivingRoomId in receivingRoomsIds)
            {
                var cultureCode = _roomsManager.GetRoomParameter(receivingRoomId, ParametersConstants.LOCALE);
                var message = string.Format(
                    _resourcesService.GetString("tour_announce_message", new CultureInfo(cultureCode)),
                    format,
                    broadcastingRoomId);
                _bot.Say(receivingRoomId, message);
            }
        }

        return Task.CompletedTask;
    }
}