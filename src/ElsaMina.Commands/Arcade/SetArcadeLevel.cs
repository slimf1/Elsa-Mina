using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Arcade;

[NamedCommand("addpalier", "setpalier")]
public class SetArcadeLevel : Command
{
    private readonly IArcadeLevelRepository _arcadeLevelRepository;

    public SetArcadeLevel(IArcadeLevelRepository arcadeLevelRepository)
    {
        _arcadeLevelRepository = arcadeLevelRepository;
    }

    public override char RequiredRank => '%';
    public override string[] AllowedRooms => ["arcade"];
    public override string HelpMessageKey => "arcade_level_help";

    public override async Task Run(IContext context)
    {
        string user;
        int level;
        try
        {
            var parts = context.Target.Split(",");
            user = parts[0].ToLowerAlphaNum();
            level = int.Parse(parts[1]);
        }
        catch (Exception)
        {
            ReplyLocalizedHelpMessage(context);
            return;
        }

        if (level is < 2 or > 4)
        {
            context.ReplyLocalizedMessage("arcade_level_invalid_value");
            return;
        }
        
        var arcadeLevel = await _arcadeLevelRepository.GetByIdAsync(user);
        if (arcadeLevel == null)
        {
            await _arcadeLevelRepository.AddAsync(new ArcadeLevel
            {
                Id = user,
                Level = level
            });
            context.ReplyLocalizedMessage("arcade_level_add", user, level);
        }
        else
        {
            arcadeLevel.Level = level;
            await _arcadeLevelRepository.UpdateAsync(arcadeLevel);
            context.ReplyLocalizedMessage("arcade_level_update", user, level);
        }
    }
}