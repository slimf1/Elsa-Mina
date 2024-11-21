using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Teams.Samples;

[NamedCommand("team-list", Aliases = ["teams"])]
public class TeamList : Command
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITemplatesManager _templatesManager;

    public TeamList(ITeamRepository teamRepository,
        ITemplatesManager templatesManager)
    {
        _teamRepository = teamRepository;
        _templatesManager = templatesManager;
    }

    public override bool IsAllowedInPrivateMessage => true;

    public override async Task Run(IContext context)
    {
        IEnumerable<Team> teams;
        string roomId;
        if (!string.IsNullOrEmpty(context.Target))
        {
            var arguments = context.Target.Split(",");
            var format = arguments[0].ToLowerAlphaNum();
            roomId = arguments.Length >= 2 ? arguments[1].ToLowerAlphaNum() : context.RoomId;
            teams = await _teamRepository.GetTeamsFromRoomWithFormat(roomId, format);
        }
        else
        {
            roomId = context.RoomId;
            teams = await _teamRepository.GetTeamsFromRoom(roomId);
        }

        var teamList = teams?.ToList();
        if (teamList == null || teamList.Count == 0)
        {
            context.ReplyLocalizedMessage("team_list_empty");
            return;
        }

        var template = await _templatesManager.GetTemplate("Teams/TeamList", new TeamListViewModel
        {
            Culture = context.Culture,
            Teams = teamList
        });

        var html = template.RemoveNewlines();
        if (context.HasSufficientRank('+'))
        {
            var message = $"""<div style="overflow-y: auto; max-height: 230px;">{html}</div>""";
            context.SendHtml(message, roomId: roomId);
        }
        else
        {
            context.SendHtmlPage($"teams-{context.RoomId}", html);
        }
    }
}