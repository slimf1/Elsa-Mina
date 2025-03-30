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
    public override string[] RoomRestriction => ["arcade", "botdevelopment"];
    public override string HelpMessageKey => "arcade_level_delete_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.Target))
        {
            ReplyLocalizedHelpMessage(context);
            return;
        }

        var id = context.Target.ToLowerAlphaNum();
        var arcadeLevel = await _arcadeLevelRepository.GetByIdAsync(id, cancellationToken);
        if (arcadeLevel == null)
        {
            context.ReplyLocalizedMessage("arcade_level_delete_not_found");
        }
        else
        {
            try
            {
                await _arcadeLevelRepository.DeleteByIdAsync(id, cancellationToken);
                context.ReplyLocalizedMessage("arcade_level_delete_success");
            }
            catch (Exception e)
            {
                context.ReplyLocalizedMessage("arcade_level_delete_failure", e.Message);
            }
        }
    }
}