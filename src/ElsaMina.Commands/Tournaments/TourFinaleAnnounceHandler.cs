using ElsaMina.Core;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using Newtonsoft.Json;

namespace ElsaMina.Commands.Tournaments;

public class TourFinaleAnnounceHandler : Handler
{
    private readonly IBot _bot;
    private readonly IResourcesService _resourcesService;
    private readonly IRoomsManager _roomsManager;

    public TourFinaleAnnounceHandler(IBot bot,
        IResourcesService resourcesService,
        IRoomsManager roomsManager)
    {
        _bot = bot;
        _resourcesService = resourcesService;
        _roomsManager = roomsManager;
    }

    public override Task HandleReceivedMessage(string[] parts, string roomId = null)
    {
        if (parts.Length < 4 || parts[1] != "tournament" || parts[2] != "update")
        {
            return Task.CompletedTask;
        }

        var tournamentData = JsonConvert.DeserializeObject<TournamentData>(parts[3]);
        if (tournamentData?.BracketData?.RootNode?.State != "inprogress")
        {
            return Task.CompletedTask;
        }

        var battleRoom = tournamentData.BracketData.RootNode.Room;
        if (battleRoom == null)
        {
            return Task.CompletedTask;
        }

        var room = _roomsManager.GetRoom(roomId);
        if (room == null)
        {
            return Task.CompletedTask;
        }

        var messageTemplate = _resourcesService.GetString("tour_finale_announce_message", room.Culture);

        _bot.Say(roomId, $"/wall {string.Format(messageTemplate, battleRoom)}");

        return Task.CompletedTask;
    }
}