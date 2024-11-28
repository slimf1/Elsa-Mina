using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Arcade;

[NamedCommand("displaypaliers", "displaypalier", "paliers", "arcadelevels")]
public class DisplayArcadeLevels : Command
{
    private readonly IArcadeLevelRepository _arcadeLevelRepository;
    private readonly ITemplatesManager _templatesManager;

    public DisplayArcadeLevels(IArcadeLevelRepository arcadeLevelRepository,
        ITemplatesManager templatesManager)
    {
        _arcadeLevelRepository = arcadeLevelRepository;
        _templatesManager = templatesManager;
    }

    public override Rank RequiredRank => Rank.Regular;
    public override string HelpMessageKey => "display_paliers_help";
    public override string[] AllowedRooms => ["arcade", "botdevelopment"];

    public override async Task Run(IContext context)
    {
        var levels = new Dictionary<int, List<string>>();
        foreach (var arcadeLevel in await _arcadeLevelRepository.GetAllAsync())
        {
            if (levels.ContainsKey(arcadeLevel.Level))
            {
                levels[arcadeLevel.Level].Add(arcadeLevel.Id);
            }
            else
            {
                levels[arcadeLevel.Level] = [arcadeLevel.Id];
            }
        }

        var template = await _templatesManager.GetTemplate("Arcade/ArcadeLevels", new ArcadeLevelsViewModel
        {
            Culture = context.Culture,
            Levels = levels
        });
        
        context.SendHtml(template.RemoveNewlines(), rankAware: true);
    }
}