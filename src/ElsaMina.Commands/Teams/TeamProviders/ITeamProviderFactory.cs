namespace ElsaMina.Commands.Teams.TeamProviders;

public interface ITeamProviderFactory
{
    IEnumerable<string> SupportedProviderLinks { get; }
    ITeamProvider GetTeamProvider(string link);
}