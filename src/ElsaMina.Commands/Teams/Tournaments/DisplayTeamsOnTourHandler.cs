using ElsaMina.Core;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Teams.Tournaments;

public class DisplayTeamsOnTourHandler : Handler
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITemplatesManager _templatesManager;
    private readonly IRoomsManager _roomsManager;
    private readonly IBot _bot;

    public DisplayTeamsOnTourHandler(ITeamRepository teamRepository,
        ITemplatesManager templatesManager,
        IRoomsManager roomsManager,
        IBot bot)
    {
        _teamRepository = teamRepository;
        _templatesManager = templatesManager;
        _roomsManager = roomsManager;
        _bot = bot;
    }

    public override async Task HandleReceivedMessage(string[] parts, string roomId = null)
    {
        if (parts.Length < 4 || parts[1] != "tournament" || parts[2] != "create")
        {
            return;
        }

        var format = parts[3].ToLowerAlphaNum();
        if (format.Contains("random"))
        {
            return;
        }

        if (format.StartsWith("gen9"))
        {
            format = format[4..];
        }

        format = format switch
        {
            // TODO : chercher à mieux gérer des aliases de format ?
            "nationaldex" => "natdex",
            "almostanyability" => "aaa",
            "anythinggoes" => "ag",
            _ => format
        };

        var teams = (await _teamRepository.GetTeamsFromRoomWithFormat(roomId, format))?.ToList();
        if (teams == null || teams.Count == 0)
        {
            return;
        }
        var room = _roomsManager.GetRoom(roomId);
        var template = await _templatesManager.GetTemplate("Teams/TeamList", new TeamListViewModel
        {
            Culture = room.Culture,
            Teams = teams
        });

        _bot.Say(roomId, $"/addhtmlbox {template.RemoveNewlines()}");
    }
}