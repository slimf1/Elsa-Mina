using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Teams.Samples;

[NamedCommand("team-showcase", Aliases = ["team"])]
public class TeamShowcase : Command
{
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
        
        var template = await _templatesManager.GetTemplate("Teams/SampleTeam", new SampleTeamViewModel
        {
            Culture = context.Culture,
            Team = team
        });

        context.SendHtml(template.RemoveNewlines(), rankAware: true);
    }
}