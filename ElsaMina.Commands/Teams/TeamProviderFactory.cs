using System.Text.RegularExpressions;
using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Commands.Teams.TeamProviders.CoupCritique;
using ElsaMina.Commands.Teams.TeamProviders.Pokepaste;
using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Commands.Teams;

public class TeamProviderFactory : ITeamProviderFactory
{
    public static readonly Regex TEAM_LINK_REGEX = new(@"https:\/\/((pokepast\.es\/[0-9A-Fa-f]{16}\/?)|(www\.coupcritique\.fr\/entity\/teams\/\d+\/?))");
    
    private const string POKEPASTE_BASE_LINK = "pokepast.es/";
    private const string COUP_CRITIQUE_BASE_LINK = "coupcritique.fr/entity/teams/";

    private readonly IDependencyContainerService _dependencyContainerService;

    public TeamProviderFactory(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }

    public IEnumerable<string> SupportedProviderLinks => new[]
    {
        POKEPASTE_BASE_LINK,
        COUP_CRITIQUE_BASE_LINK
    };

    public ITeamProvider GetTeamProvider(string link)
    {
        if (link.Contains(POKEPASTE_BASE_LINK))
        {
            return _dependencyContainerService.Resolve<PokepasteProvider>();
        }

        if (link.Contains(COUP_CRITIQUE_BASE_LINK))
        {
            return _dependencyContainerService.Resolve<CoupCritiqueProvider>();
        }

        throw new ArgumentException();
    }
}