using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Teams.Samples;

public class TeamList : Command<TeamList>, INamed
{
    public static string Name => "team-list";
    public static List<string> Aliases => ["teams"];

    private readonly ITeamRepository _teamRepository;
    private readonly ITemplatesManager _templatesManager;

    public TeamList(ITeamRepository teamRepository,
        ITemplatesManager templatesManager)
    {
        _teamRepository = teamRepository;
        _templatesManager = templatesManager;
    }

    public override async Task Run(IContext context)
    {
        var teams = await _teamRepository.GetTeamsFromRoom(context.RoomId);
        var teamList = teams?.ToList();
        if (teamList == null || teamList.Count == 0)
        {
            // TODO
            return;
        }

        var template = await _templatesManager.GetTemplate("Teams/TeamList", new TeamListViewModel
        {
            Culture = context.Culture,
            Teams = teamList
        });
        
        context.SendHtmlPage($"teams-{context.RoomId}", template.RemoveNewlines());
    }
}