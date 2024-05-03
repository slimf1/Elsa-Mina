namespace ElsaMina.Commands.Teams.TeamProviders;

public class TeamLinkMatch : ITeamLinkMatch
{
    public ITeamProvider Provider { get; init; }
    public string TeamLink { get; init; }

    public async Task<SharedTeam> GetTeamExport()
    {
        if (Provider == null || TeamLink == null)
        {
            return null;
        }

        return await Provider.GetTeamExport(TeamLink);
    }
}