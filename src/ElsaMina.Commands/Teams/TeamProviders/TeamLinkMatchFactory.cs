using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Commands.Teams.TeamProviders;

public class TeamLinkMatchFactory : ITeamLinkMatchFactory
{
    private readonly IDependencyContainerService _dependencyContainerService;
    private IEnumerable<ITeamProvider> _teamProviders;

    public TeamLinkMatchFactory(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }
    
    public ITeamLinkMatch FindTeamLinkMatch(string message)
    {
        _teamProviders ??= _dependencyContainerService.Resolve<IEnumerable<ITeamProvider>>();
        return _teamProviders
            .Select(provider => GetTeamLinkMatch(message, provider))
            .FirstOrDefault(match => match != null);
    }

    private static TeamLinkMatch GetTeamLinkMatch(string message, ITeamProvider provider)
    {
        var matchingLink = provider.GetMatchFromLink(message);
        if (!string.IsNullOrEmpty(matchingLink))
        {
            return new TeamLinkMatch
            {
                Provider = provider,
                TeamLink = matchingLink
            };
        }

        return null;
    }
}