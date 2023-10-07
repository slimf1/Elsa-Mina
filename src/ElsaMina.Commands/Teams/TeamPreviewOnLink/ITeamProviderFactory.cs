using ElsaMina.Commands.Teams.TeamPreviewOnLink.TeamProviders;

namespace ElsaMina.Commands.Teams.TeamPreviewOnLink;

public interface ITeamProviderFactory
{
    IEnumerable<string> SupportedProviderLinks { get; }
    ITeamProvider GetTeamProvider(string link);
}