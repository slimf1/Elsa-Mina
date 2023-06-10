using ElsaMina.Commands.Teams.TeamProviders;

namespace ElsaMina.Commands.Teams;

public interface ITeamProviderFactory
{
    ITeamProvider GetTeamProvider(string link);
}