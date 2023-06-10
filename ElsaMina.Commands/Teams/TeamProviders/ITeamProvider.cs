namespace ElsaMina.Commands.Teams.TeamProviders;

public interface ITeamProvider
{ 
    Task<SharedTeam> GetTeamExport(string teamLink);
}