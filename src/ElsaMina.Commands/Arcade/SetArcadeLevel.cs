using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
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

    public override Rank RequiredRank => Rank.Driver;
    public override string[] RoomRestriction => ["arcade", "botdevelopment"];
    public override string HelpMessageKey => "arcade_level_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
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
        
        var arcadeLevel = await _arcadeLevelRepository.GetByIdAsync(user, cancellationToken);
        if (arcadeLevel == null)
        {
            try
            {
                await _arcadeLevelRepository.AddAsync(new ArcadeLevel
                {
                    Id = user,
                    Level = level
                }, cancellationToken);
                context.ReplyLocalizedMessage("arcade_level_add", user, level);
                await _arcadeLevelRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                context.ReplyLocalizedMessage("arcade_level_update_error", e.Message);
            }
        }
        else
        {
            arcadeLevel.Level = level;
            try
            {
                await _arcadeLevelRepository.SaveChangesAsync(cancellationToken);
                context.ReplyLocalizedMessage("arcade_level_update", user, level);
            }
            catch (Exception e)
            {
                context.ReplyLocalizedMessage("arcade_level_update_error", e.Message);
            }
        }
    }
}