namespace ElsaMina.Commands.Teams.TeamProviders;

public interface ITeamLinkMatch
{
    ITeamProvider Provider { get; set; }
    string TeamLink { get; set; }
    Task<SharedTeam> GetTeamExport();
}