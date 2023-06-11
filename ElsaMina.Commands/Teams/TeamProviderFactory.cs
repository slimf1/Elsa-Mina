using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Commands.Teams.TeamProviders.CoupCritique;
using ElsaMina.Commands.Teams.TeamProviders.Pokepaste;
using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Commands.Teams;

public class TeamProviderFactory : ITeamProviderFactory
{
    private readonly IDependencyContainerService _dependencyContainerService;

    public TeamProviderFactory(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }

    public ITeamProvider GetTeamProvider(string link)
    {
        if (link.Contains("pokepast.es/"))
        {
            return _dependencyContainerService.Resolve<PokepasteProvider>();
        }

        if (link.Contains("coupcritique.fr/entity/teams/"))
        {
            return _dependencyContainerService.Resolve<CoupCritiqueProvider>();
        }

        throw new ArgumentException();
    }
}