using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Teams.Samples;

public class DeleteTeam : Command<DeleteTeam>, INamed
{
    public static string Name => "delete-team";
    public static List<string> Aliases => ["deleteteam"];
    
    private readonly ITeamRepository _teamRepository;

    public DeleteTeam(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public override char RequiredRank => '+';

    public override async Task Run(IContext context)
    {
        var team = await _teamRepository.GetByIdAsync(context.Target?.ToLowerAlphaNum());
        if (team == null)
        {
            context.ReplyLocalizedMessage("deleteteam_team_not_found");
            return;
        }

        try
        {
            await _teamRepository.DeleteAsync(team.Id);
            context.ReplyLocalizedMessage("deleteteam_team_deleted_successfully");
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "An error occurred while deleting team");
            context.ReplyLocalizedMessage("deleteteam_team_deletion_error", exception.Message);
        }
    }
}