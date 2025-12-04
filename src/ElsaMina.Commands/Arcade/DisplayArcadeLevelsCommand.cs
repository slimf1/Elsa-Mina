using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using ElsaMina.Logging;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.Arcade;

[NamedCommand("displaypaliers", "displaypalier", "paliers", "arcadelevels")]
public class DisplayArcadeLevelsCommand : Command
{
    private readonly ITemplatesManager _templatesManager;
    private readonly IBotDbContextFactory _dbContextFactory;

    public DisplayArcadeLevelsCommand(ITemplatesManager templatesManager, IBotDbContextFactory dbContextFactory)
    {
        _templatesManager = templatesManager;
        _dbContextFactory = dbContextFactory;
    }

    public override Rank RequiredRank => Rank.Regular;
    public override string HelpMessageKey => "display_paliers_help";
    public override string[] RoomRestriction => ["arcade", "botdevelopment"];

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var levels = new Dictionary<int, List<string>>();

        List<ArcadeLevel> arcadeLevels = [];
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        try
        {
            arcadeLevels = await dbContext.ArcadeLevels.ToListAsync(cancellationToken);
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