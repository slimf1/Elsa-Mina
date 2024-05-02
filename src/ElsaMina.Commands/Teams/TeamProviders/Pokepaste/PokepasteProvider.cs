using System.Text.RegularExpressions;
using ElsaMina.Core;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Teams.TeamProviders.Pokepaste;

public partial class PokepasteProvider : ITeamProvider
{
    private static readonly Regex TEAM_LINK_REGEX = TeamLinkRegex();
    
    private readonly IHttpService _httpService;

    public PokepasteProvider(IHttpService httpService)
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
            var pokepasteTeam = await _httpService.Get<PokepasteTeam>(teamLink.Trim() + "/json");
            return new SharedTeam
            {
                Title = pokepasteTeam.Title,
                Description = pokepasteTeam.Notes,
                Author = pokepasteTeam.Author,
                TeamExport = pokepasteTeam.Paste
            };
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "An error occurred while fetching a postepaste team");
            return null;
        }
    }

    [GeneratedRegex(@"https:\/\/(pokepast\.es\/[0-9A-Fa-f]{16}\/?)")]
    private static partial Regex TeamLinkRegex();
}