namespace ElsaMina.Commands.Teams.TeamProviders;

public interface ITeamLinkMatchFactory
{
    ITeamLinkMatch FindTeamLinkMatch(string message);
}