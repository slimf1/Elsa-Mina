using System.Text.RegularExpressions;
using ElsaMina.Core;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Teams.TeamProviders.Pokepaste;

public class PokepasteProvider : ITeamProvider
{
    private static readonly Regex TEAM_LINK_REGEX = new(@"https:\/\/(pokepast\.es\/[0-9A-Fa-f]{16}\/?)",
        RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    
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
            var response = await _httpService.Get<PokepasteTeam>(teamLink.Trim() + "/json");
            var pokepasteTeam = response.Data;
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
            Logger.Error(exception, "An error occurred while fetching a postepaste team");
            return null;
        }
    }
}