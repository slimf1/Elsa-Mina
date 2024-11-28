using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Arcade;

[NamedCommand("deletepalier", "removepalier", "removelevel")]
public class DeleteArcadeLevel : Command
{
    private readonly IArcadeLevelRepository _arcadeLevelRepository;

    public DeleteArcadeLevel(IArcadeLevelRepository arcadeLevelRepository)
    {
        _arcadeLevelRepository = arcadeLevelRepository;
    }
    
    public override Rank RequiredRank => Rank.Driver;
    public override string[] AllowedRooms => ["arcade", "botdevelopment"];
    public override string HelpMessageKey => "arcade_level_delete_help";

    public override async Task Run(IContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Target))
        {
            ReplyLocalizedHelpMessage(context);
            return;
        }

        var id = context.Target.ToLowerAlphaNum();
        var arcadeLevel = await _arcadeLevelRepository.GetByIdAsync(id);
        if (arcadeLevel == null)
        {
            context.ReplyLocalizedMessage("arcade_level_delete_not_found");
        }
        else
        {
            await _arcadeLevelRepository.DeleteAsync(id);
            context.ReplyLocalizedMessage("arcade_level_delete_success");
        }
    }
}