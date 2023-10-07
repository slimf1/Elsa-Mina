namespace ElsaMina.Commands.Teams.TeamPreviewOnLink.TeamProviders;

public interface ITeamProvider
{ 
    Task<SharedTeam> GetTeamExport(string teamLink);
}