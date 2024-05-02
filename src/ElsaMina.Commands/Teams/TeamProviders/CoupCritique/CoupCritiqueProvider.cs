using System.Text.RegularExpressions;
using ElsaMina.Core;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Teams.TeamProviders.CoupCritique;

public partial class CoupCritiqueProvider : ITeamProvider
{
    private const string COUP_CRITIQUE_API_URL = "https://www.coupcritique.fr/api/teams/{0}";

    private static readonly Regex TEAM_LINK_REGEX = TeamLinkRegex();
    
    private readonly IHttpService _httpService;

    public CoupCritiqueProvider(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public string GetMatchFromLink(string teamLink)
    {
        var match = TEAM_LINK_REGEX.Match(teamLink);
        return match.Success ? match.Value : null;
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
            Logger.Current.Error(exception, "An error occurred while fetching team from Coup Critique");
            return null;
        }
    }

    [GeneratedRegex(@"https:\/\/(www\.coupcritique\.fr\/entity\/teams\/\d+\/?)")]
    private static partial Regex TeamLinkRegex();
}