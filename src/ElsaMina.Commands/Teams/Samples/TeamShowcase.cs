using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;

namespace ElsaMina.Commands.Teams.Samples;

[NamedCommand("team-showcase", Aliases = ["team"])]
public class TeamShowcase : Command
{
    private readonly ITemplatesManager _templatesManager;
    private readonly IBotDbContextFactory _dbContextFactory;

    public TeamShowcase(ITemplatesManager templatesManager, IBotDbContextFactory dbContextFactory)
    {
        _templatesManager = templatesManager;
        _dbContextFactory = dbContextFactory;
    }

    public override Rank RequiredRank => Rank.Regular;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var team = await dbContext.Teams.FindAsync([context.Target?.ToLowerAlphaNum()], cancellationToken);
        if (team == null)
        {
            context.ReplyLocalizedMessage("team_showcase_not_found");
            return;
        }

        var template = await _templatesManager.GetTemplateAsync("Teams/SampleTeam", new SampleTeamViewModel
        {
            Culture = context.Culture,
            Team = team
        });

        context.ReplyHtml(template.RemoveNewlines(), rankAware: true);
    }
}