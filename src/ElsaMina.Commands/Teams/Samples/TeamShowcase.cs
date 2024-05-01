using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Templates.SampleTeam;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Teams.Samples;

public class TeamShowcase : Command<TeamShowcase>, INamed
{
    public static string Name => "team-showcase";
    public static IEnumerable<string> Aliases => new[] { "team" };

    private readonly ITeamRepository _teamRepository;
    private readonly ITemplatesManager _templatesManager;

    public TeamShowcase(ITeamRepository teamRepository,
        ITemplatesManager templatesManager)
    {
        _teamRepository = teamRepository;
        _templatesManager = templatesManager;
    }

    public override async Task Run(IContext context)
    {
        var team = await _teamRepository.GetByIdAsync(context.Target?.ToLowerAlphaNum());
        if (team == null)
        {
            context.ReplyLocalizedMessage("team_showcase_not_found");
            return;
        }
        
        var template = await _templatesManager.GetTemplate("SampleTeam/SampleTeam", new SampleTeamViewModel
        {
            Culture = context.Culture,
            Team = team
        });

        // TODO : if sur le rank (page if < voiced)
        context.SendHtml(template.RemoveNewlines());
    }
}