using ElsaMina.Commands.Teams.TeamProviders;

namespace ElsaMina.Commands.Teams;

public interface ITeamProviderFactory
{
    IEnumerable<string> SupportedProviderLinks { get; }
    ITeamProvider GetTeamProvider(string link);
}