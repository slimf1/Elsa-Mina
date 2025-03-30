namespace ElsaMina.Commands.Teams.TeamProviders;

public interface ITeamProvider
{
    string GetMatchFromLink(string teamLink);
    Task<SharedTeam> GetTeamExport(string teamLink, CancellationToken cancellationToken = default);
}