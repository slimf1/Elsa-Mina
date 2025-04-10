using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
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
    public override string[] RoomRestriction => ["arcade", "botdevelopment"];

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var levels = new Dictionary<int, List<string>>();

        List<ArcadeLevel> arcadeLevels = [];
        try
        {
            arcadeLevels = (await _arcadeLevelRepository.GetAllAsync(cancellationToken)).ToList();
        }
        catch (Exception e)
        {
            Log.Error(e, "An error occurred while getting arcade levels.");
        }

        if (arcadeLevels.Count == 0)
        {
            context.ReplyLocalizedMessage("arcade_level_no_users");
            return;
        }

        foreach (var arcadeLevel in arcadeLevels)
        {
            if (levels.TryGetValue(arcadeLevel.Level, out var currentLevelList))
            {
                currentLevelList.Add(arcadeLevel.Id);
            }
            else
            {
                levels[arcadeLevel.Level] = [arcadeLevel.Id];
            }
        }

        var template = await _templatesManager.GetTemplateAsync("Arcade/ArcadeLevels", new ArcadeLevelsViewModel
        {
            Culture = context.Culture,
            Levels = levels
        });

        context.ReplyHtml(template.RemoveNewlines(), rankAware: true);
    }
}