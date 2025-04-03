using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Teams.Samples;

[NamedCommand("delete-team", Aliases = ["deleteteam"])]
public class DeleteTeam : Command
{
    private readonly ITeamRepository _teamRepository;

    public DeleteTeam(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(context.Target?.ToLowerAlphaNum(), cancellationToken);
        if (team == null)
        {
            context.ReplyLocalizedMessage("deleteteam_team_not_found");
            return;
        }

        try
        {
            await _teamRepository.DeleteByIdAsync(team.Id, cancellationToken);
            context.ReplyLocalizedMessage("deleteteam_team_deleted_successfully");
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred while deleting team");
            context.ReplyLocalizedMessage("deleteteam_team_deletion_error", exception.Message);
        }
    }
}