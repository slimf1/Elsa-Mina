using ElsaMina.Core.Services.Http;
using Serilog;

namespace ElsaMina.Commands.Teams.TeamPreviewOnLink.TeamProviders.CoupCritique;

public class CoupCritiqueProvider : ITeamProvider
{
    private const string COUP_CRITIQUE_API_URL = "https://www.coupcritique.fr/api/teams/{0}";
    
    private readonly ILogger _logger;
    private readonly IHttpService _httpService;

    public CoupCritiqueProvider(ILogger logger, IHttpService httpService)
    {
        _logger = logger;
        _httpService = httpService;
    }

    public async Task<SharedTeam> GetTeamExport(string teamLink)
    {
        try
        {
            if (teamLink.Last() == '/')
            {
                teamLink = teamLink.Remove(teamLink.Length - 1);
            }

            var teamId = teamLink.Split("/").Last();
            var response = await _httpService.Get<CoupCritiqueResponse>(string.Format(COUP_CRITIQUE_API_URL, teamId));
            return new SharedTeam
            {
                Description = response.Team.Description,
                TeamExport = response.Team.Export,
                Author = response.Team.User.UserName,
                Title = response.Team.Name
            };
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "An error occurred while fetching team from Coup Critique");
            return null;
        }
    }
}