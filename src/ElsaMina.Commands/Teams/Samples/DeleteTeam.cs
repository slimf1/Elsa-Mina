using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;
using Serilog;

namespace ElsaMina.Commands.Teams.Samples;

public class DeleteTeam : Command<DeleteTeam>, INamed
{
    public static string Name => "delete-team";
    public static IEnumerable<string> Aliases => new[] { "deleteteam" };
    
    private readonly ITeamRepository _teamRepository;
    private readonly ILogger _logger;

    public DeleteTeam(ITeamRepository teamRepository,
        ILogger logger)
    {
        _teamRepository = teamRepository;
        _logger = logger;
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
            _logger.Error(exception, "An error occurred while deleting team");
            context.ReplyLocalizedMessage("deleteteam_team_deletion_error", exception.Message);
        }
    }
}